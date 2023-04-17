using System.Collections.Immutable;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace KiloTx.Restful.ClientGenerator;

[Generator(LanguageNames.CSharp)]
public class RestfulSourceGenerator : IIncrementalGenerator
{
    private static readonly Dictionary<string, string> _httpMethods = new()
    {
        { "KiloTx.Restful.ApiGetAttribute", "HttpMethod.Get" },
        { "KiloTx.Restful.ApiPostAttribute", "HttpMethod.Post" },
        { "KiloTx.Restful.ApiDeleteAttribute", "HttpMethod.Delete" },
        { "KiloTx.Restful.ApiPutAttribute", "HttpMethod.Put" },
        { "KiloTx.Restful.ApiPatchAttribute", "HttpMethod.Patch" }
    };

    private static bool IsRestfullImplementation(SyntaxNode syntaxNode, CancellationToken cancellationToken)
    {
        if (syntaxNode is not AttributeSyntax attribute)
            return false;

        var name = attribute.Name switch
        {
            SimpleNameSyntax ins => ins.Identifier.Text,
            QualifiedNameSyntax qns => qns.Right.Identifier.Text,
            _ => null
        };

        return name is "RestfulImplementation" or "RestfulImplementationAttribute";
    }

    private static ITypeSymbol? GetTypeFromAttribute(GeneratorSyntaxContext context,
        CancellationToken cancellationToken)
    {
        var attributeSyntax = (AttributeSyntax)context.Node;

        // "attribute.Parent" is "AttributeListSyntax"
        // "attribute.Parent.Parent" is a C# fragment the attributes are applied to
        TypeDeclarationSyntax? typeNode = attributeSyntax.Parent?.Parent switch
        {
            ClassDeclarationSyntax classDeclarationSyntax => classDeclarationSyntax,
            RecordDeclarationSyntax recordDeclarationSyntax => recordDeclarationSyntax,
            StructDeclarationSyntax structDeclarationSyntax => structDeclarationSyntax,
            _ => null
        };

        if (typeNode == null)
            return null;

        if (context.SemanticModel.GetDeclaredSymbol(typeNode) is not ITypeSymbol type)
            return null;

        return type;
    }

    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var types = context.SyntaxProvider.CreateSyntaxProvider(IsRestfullImplementation, GetTypeFromAttribute)
            .Where(t => t != null)
            .Collect();

        context.RegisterSourceOutput(types, GenerateSource!);
    }

    private static void GenerateSource(SourceProductionContext context, ImmutableArray<ITypeSymbol> types)
    {
        if (types.IsDefaultOrEmpty)
            return;

        foreach (var type in types)
        {
            context.CancellationToken.ThrowIfCancellationRequested();

            var source = GenerateSourceCode(type);

            var hintSymbolDisplayFormat = new SymbolDisplayFormat(
                globalNamespaceStyle: SymbolDisplayGlobalNamespaceStyle.Omitted,
                typeQualificationStyle: SymbolDisplayTypeQualificationStyle.NameAndContainingTypesAndNamespaces,
                genericsOptions: SymbolDisplayGenericsOptions.IncludeTypeParameters,
                miscellaneousOptions:
                SymbolDisplayMiscellaneousOptions.EscapeKeywordIdentifiers |
                SymbolDisplayMiscellaneousOptions.UseSpecialTypes);

            var hintName = type.ToDisplayString(hintSymbolDisplayFormat)
                .Replace('<', '[')
                .Replace('>', ']');

            context.AddSource($"{hintName}.g.cs", source);
        }
    }

    private static SourceText GenerateSourceCode(ITypeSymbol type)
    {
        var interfaces = GetInterfaces(type);

        var implements = string.Join(", ",
            interfaces.Select(x => x.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat)));

        var usings = new HashSet<string>() { "System.Text.Json", "System.Net.Http.Json", "KiloTx.Restful" };
        usings.AddRange(interfaces.Select(interfaceType => interfaceType.ContainingNamespace.ToDisplayString()));

        var source = new CodeBuilder();
        source.AppendLine();
        source.AppendLine($"namespace {type.ContainingNamespace};");
        source.AppendLine();
        source.AppendLine($"partial class {type.Name} : {implements}");
        source.OpenBlock();

        source.AppendLine("private HttpClient _httpClient;");
        source.AppendLine();
        source.AppendLine($"public {type.Name}(HttpClient httpClient)");
        source.OpenBlock();
        source.AppendLine("_httpClient = httpClient;");
        source.CloseBlock();
        source.AppendLine();

        source.AppendLine("private static readonly JsonSerializerOptions JsonOptions = new()")
            .OpenBlock()
            .AppendLine("Converters = { new System.Text.Json.Serialization.JsonStringEnumConverter() },")
            .AppendLine("WriteIndented = true,")
            .AppendLine("PropertyNamingPolicy = JsonNamingPolicy.CamelCase")
            .CloseBlock(";");
        source.AppendLine();

        var argBuilder = new StringBuilder();

        foreach (var implementType in interfaces)
        {
            // Get the root uri
            var apiRootAttribute = implementType.GetAttributes()
                .FirstOrDefault(x => x.AttributeClass!.Name == nameof(ApiRootAttribute));
            if (apiRootAttribute == null) continue;
            var apiRoot = ((string)apiRootAttribute.ConstructorArguments.First().Value!).TrimEnd('/');

            var members = implementType.GetMembers()
                .Select(x => x as IMethodSymbol).Where(x => x != null);
            foreach (var member in members)
            {
                if (member == null || member.IsStatic || member.MethodKind != MethodKind.Ordinary) continue;

                try
                {
                    // using for Return types
                    usings.AddRange(GetUsings(member.ReturnType));

                    // Extract response type
                    var taskType = member.ReturnType as INamedTypeSymbol ??
                                   throw new InvalidOperationException(
                                       $"Method {member.Name} must have a Task return type");
                    // TODO -- expect Task
                    var returnType = taskType.IsGenericType ? taskType.TypeArguments.Single() : null;

                    // using for method Parameters
                    foreach (var argument in member.Parameters)
                    {
                        usings.AddRange(GetUsings(argument.Type));
                    }

                    // Aggregate any ApiPathAttribute
                    var apiPaths =
                        from attribute in member.GetAttributes()
                        let attributeName = attribute?.AttributeClass?.ToDisplayString() ?? ""
                        where _httpMethods.ContainsKey(attributeName)
                        select new
                        {
                            Method = _httpMethods.GetValueOrDefault(attributeName),
                            Path = attribute.ConstructorArguments.Single().Value as string
                        };

                    // Get first ApiPathAttribute
                    var apiPath =
                        apiPaths.FirstOrDefault()
                        ?? throw new InvalidOperationException($"Method {member.Name} must have ApiRouteAttribute");

                    // DEBUG -- Show the attribute
                    // source.AppendLines(member.GetAttributes().Select(x =>
                    //     $"// [{x.AttributeClass.ToDisplayString()}](\"{x.ConstructorArguments.First().Value}\")"));

                    // List the method arguments
                    var arguments = string.Join(", ", member.Parameters
                        .Select(x => x.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat)));

                    // OUTPUT - Method declaration
                    source.AppendLine(
                        $@"async {member.ReturnType.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat)} {implementType.Name}.{member.Name}({arguments})");
                    source.OpenBlock();

                    // Aggregate the argument names
                    var contentArgs = member.Parameters
                        .Select(x => x.ToDisplayParts(SymbolDisplayFormat.MinimallyQualifiedFormat).Last().ToString())
                        .ToHashSet();

                    // Strip out the path arguments from argument names
                    var url = apiPath.Path;
                    if (!url.StartsWith('/'))
                        url = $"{apiRoot}/{url}";
                    var urlArgs = Regex.Matches(url, @"(\{(\w+)\??(:[^}]*)?\})", RegexOptions.Compiled)
                        .Select(match => match.Groups[2].Value)
                        .ToList();
                    contentArgs.RemoveRange(urlArgs);

                    var hasBody = apiPath.Method != "HttpMethod.Get";
                    var hasData = contentArgs.Count > 0;
                    var fileParameter = member.Parameters.FirstOrDefault(parameter => parameter.Type.Name == "Stream");

                    if (fileParameter != null)
                    {
                        var parameterName = fileParameter.ToDisplayParts(SymbolDisplayFormat.MinimallyQualifiedFormat)
                            .Last().ToString();

                        var hasMediaType = member.Parameters.Any(parameter => parameter.Name == "mediaType");
                        var hasFileName = member.Parameters.Any(parameter => parameter.Name == "fileName");

                        if (hasMediaType)
                        {
                            source.AppendLine($"var httpStreamContent = new StreamContent({parameterName}) "
                                              + "{ Headers = { ContentType = new MediaTypeHeaderValue(mediaType) }};");
                            usings.Add("System.Net.Http.Headers");
                        }
                        else
                        {
                            source.AppendLine($"var httpStreamContent = new StreamContent({parameterName});");
                        }

                        source.AppendLine(
                                @$"var httpMessage = new HttpRequestMessage({apiPath.Method}, $""{url}"")")
                            .AppendLine("{").IncreaseIndent()
                            .AppendLine(
                                $@"Content = new MultipartFormDataContent {{ {{ httpStreamContent, ""file"", {(hasFileName ? "fileName" : @"""unnamed""")} }} }}")
                            .DecreaseIndent().AppendLine("};");
                    }
                    else if (hasData)
                    {
                        if (!hasBody)
                        {
                            argBuilder.Clear();
                            argBuilder.Append(url).Append("?");
                            foreach (var arg in contentArgs)
                            {
                                argBuilder.Append($"{arg}={{Uri.EscapeDataString({arg}.ToString())}}&");
                            }
                            argBuilder.Remove(argBuilder.Length - 1, 1);

                            source.AppendLine($@"var messageUrl = $""{argBuilder}"";");
                            source.AppendLine(
                                @$"var httpMessage = new HttpRequestMessage({apiPath.Method}, messageUrl);");
                        }
                        else
                        {
                            source.AppendLine(
                                @$"var httpMessage = new HttpRequestMessage({apiPath.Method}, $""{url}"");");
                            var argumentsNames = string.Join(", ", contentArgs);
                            source.AppendLine($"var httpContent = new {{ {argumentsNames} }};");
                            if (hasBody)
                                source.AppendLine("httpMessage.Content = JsonContent.Create(httpContent);");
                        }
                    }
                    else
                    {
                        source.AppendLine(@$"var httpMessage = new HttpRequestMessage({apiPath.Method}, $""{url}"");");
                        if (hasBody)
                            source.AppendLine(@"httpMessage.Content = new StringContent("""");");
                    }

                    if (returnType != null)
                    {
                        var returnTypeName = returnType.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat);
                        source
                            .AppendLine(@$"var httpResponse = await _httpClient.SendAsync(httpMessage);")
                            .AppendLine(
                                @"await using var httpResponseStream = await httpResponse.Content.ReadAsStreamAsync();")
                            .AppendLine(
                                @$"return await JsonSerializer.DeserializeAsync<{returnTypeName}>(httpResponseStream, JsonOptions)")
                            .IncreaseIndent().AppendLine(@$"?? throw new BadRequestException(""Unable to deserialize to {returnTypeName}"");").DecreaseIndent();
                    }
                    else
                    {
                        source.AppendLine(@$"await _httpClient.SendAsync(httpMessage);");
                    }

                    source.CloseBlock();
                }
                catch (Exception ex)
                {
                    Console.Error.WriteLine(ex);
                }
            }
        }

        source.CloseBlock();

        source.PrependLines(usings.Order()
            .Where(namespaceName => !namespaceName.Contains("global namespace"))
            .Select(namespaceName => $"using {namespaceName};"));
        source.PrependLine();
        source.PrependLine("#nullable enable");

        return source;
    }

    private static IEnumerable<string> GetUsings(ISymbol? symbol)
    {
        if (symbol?.ContainingNamespace == null) yield break;

        yield return symbol.ContainingNamespace.ToDisplayString();

        if (symbol is not INamedTypeSymbol { IsGenericType: true } genericType) yield break;

        foreach (var namespaceName in genericType.TypeArguments.SelectMany(GetUsings))
            yield return namespaceName;
    }

    private static ImmutableArray<ITypeSymbol> GetInterfaces(ISymbol type)
    {
        return type.GetAttributes()
            .Where(x => x.AttributeClass?.Name == nameof(RestfulImplementationAttribute))
            .Select(x => x.AttributeClass!.TypeArguments.Single())
            .ToImmutableArray();
    }
}