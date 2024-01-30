using System.Collections.Immutable;
using System.Text;
using System.Text.RegularExpressions;
using KiloTx.Restful.ClientGenerator.Extensions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using static KiloTx.Restful.ClientGenerator.Helpers;

namespace KiloTx.Restful.ClientGenerator;

[Generator(LanguageNames.CSharp)]
public class RestfulSourceGenerator : IIncrementalGenerator
{
    private static readonly Dictionary<string, string> HttpMethods = new()
    {
        { "KiloTx.Restful.ApiGetAttribute", "HttpMethod.Get" },
        { "KiloTx.Restful.ApiPostAttribute", "HttpMethod.Post" },
        { "KiloTx.Restful.ApiDeleteAttribute", "HttpMethod.Delete" },
        { "KiloTx.Restful.ApiPutAttribute", "HttpMethod.Put" },
        { "KiloTx.Restful.ApiPatchAttribute", "HttpMethod.Patch" }
    };

    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var types =
            context.SyntaxProvider.CreateSyntaxProvider(IsRestfullImplementation, Helpers.GetTypeFromAttribute)
                .Where(t => t != null)
                .Collect();

        context.RegisterSourceOutput(types, GenerateSource!);
    }

    private static bool IsRestfullImplementation(SyntaxNode syntaxNode, CancellationToken cancellationToken) =>
        syntaxNode is AttributeSyntax attribute
        && attribute.Name switch
        {
            SimpleNameSyntax ins => ins.Identifier.Text,
            QualifiedNameSyntax qns => qns.Right.Identifier.Text,
            _ => null
        } is "RestfulImplementation" or "RestfulImplementationAttribute";

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
    
    class TypeSymbolComparer : IEqualityComparer<ITypeSymbol>
    {
        public bool Equals(ITypeSymbol x, ITypeSymbol y)
        {
            return x.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat).Equals(y.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat));
        }

        public int GetHashCode(ITypeSymbol obj)
        {
            return obj.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat).GetHashCode();
        }
    }

    private static SourceText GenerateSourceCode(ITypeSymbol type)
    {
        var interfaces = GetInterfaces(type);

        var implements = string.Join(", ",
            interfaces.Select(x => x.Interface.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat)));

        var usings = new HashSet<string>() { "System.Text.Json", "System.Net.Http.Json", "KiloTx.Restful" };
        usings.AddRange(
            interfaces.Select(interfaceType => interfaceType.Interface.ContainingNamespace.ToDisplayString()));
        usings.AddRange(interfaces.Select(interfaceType =>
            interfaceType.HttpClientFactory.ContainingNamespace.ToDisplayString()));

        var factoryTypes = interfaces
            .Select(factoryType => factoryType.HttpClientFactory)
            .Distinct(new TypeSymbolComparer())
            .OrderBy(symbol => symbol.ToDisplayString())
            .Select(symbol => (
                Symbol: symbol,
                QualifiedName: symbol.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat),
                TypeName: symbol.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat),
                ArgName: ToCamelCase(symbol.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat)),
                FieldName: $"_{ToCamelCase(symbol.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat))}"))
            .ToList();
        string[] items = factoryTypes.Select(factoryType => $"{factoryType.TypeName} {factoryType.ArgName}").ToArray();
        var factoryTypesArg = string.Join(", ", items);

        var source = new SourceCodeBuilder();
        source.AppendLine();
        source.AppendLine($"namespace {type.ContainingNamespace};");
        source.AppendLine();
        source.AppendLine($"partial class {type.Name} : {implements}");
        source.OpenBlock();

        foreach (var factoryType in factoryTypes)
            source.AppendLine($"private {factoryType.TypeName} {factoryType.FieldName};");
        source.AppendLine();

        source.AppendLine($"public {type.Name}({factoryTypesArg})");
        source.OpenBlock();
        foreach (var factoryType in factoryTypes)
        {
            source.AppendLine($"{factoryType.FieldName} = {factoryType.ArgName};");
        }

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
            var apiRootAttribute = implementType.Interface.GetAttributes()
                .FirstOrDefault(x => x.AttributeClass!.Name == nameof(ApiRootAttribute));
            if (apiRootAttribute == null) continue;
            var apiRoot = ((string)apiRootAttribute.ConstructorArguments.First().Value!).TrimEnd('/');

            var httpClient = factoryTypes.Single(x =>
                    x.QualifiedName ==
                    implementType.HttpClientFactory.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat))
                .FieldName;
            httpClient =
                $"{httpClient}.CreateHttpClient(typeof({implementType.Interface.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat)}))";

            var members = implementType.Interface.GetMembers()
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
                    var returnType = taskType.IsGenericType ? taskType.TypeArguments.Single() : null;
                    var returnTypeName = returnType?.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat);

                    // using for method Parameters
                    foreach (var argument in member.Parameters)
                    {
                        usings.AddRange(GetUsings(argument.Type));
                    }

                    // Aggregate any ApiPathAttribute
                    var apiPaths =
                        from attribute in member.GetAttributes()
                        let attributeName = attribute?.AttributeClass?.ToDisplayString() ?? ""
                        where HttpMethods.ContainsKey(attributeName)
                        select new
                        {
                            Method = HttpMethods.GetValueOrDefault(attributeName),
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
                        $@"async {member.ReturnType.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat)} {implementType.Interface.Name}.{member.Name}({arguments})");
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
                    var hasFileReturn = returnTypeName == "IFileInfo";

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

                    if (hasFileReturn)
                    {
                        source
                            .AppendLine(@$"var httpResponse = await {httpClient}.SendAsync(httpMessage);")
                            .AppendLine("httpResponse.EnsureSuccessStatusCode();")
                            .AppendLine("var httpResponseBytes = await httpResponse.Content.ReadAsByteArrayAsync();")
                            .AppendLine("var httpResponseStream = new MemoryStream(httpResponseBytes);")
                            .AppendLine(
                                @"return new DownloadFileInfo(httpResponseStream, ""download"", httpResponseBytes.Length);");
                    }
                    else if (returnType != null)
                    {
                        source
                            .AppendLine(@$"var httpResponse = await {httpClient}.SendAsync(httpMessage);")
                            .AppendLine("httpResponse.EnsureSuccessStatusCode();")
                            .AppendLine(
                                @"await using var httpResponseStream = await httpResponse.Content.ReadAsStreamAsync();")
                            .AppendLine(
                                @$"return await JsonSerializer.DeserializeAsync<{returnTypeName}>(httpResponseStream, JsonOptions)")
                            .IncreaseIndent()
                            .AppendLine(
                                @$"?? throw new BadRequestException(""Unable to deserialize to {returnTypeName}"");")
                            .DecreaseIndent();
                    }
                    else
                    {
                        source.AppendLine(@$"var httpResponse = await {httpClient}.SendAsync(httpMessage);")
                            .AppendLine("httpResponse.EnsureSuccessStatusCode();");
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

        source.PrependLines(usings.OrderBy(namespaceName => namespaceName)
            .Where(namespaceName => !namespaceName.Contains("global namespace"))
            .Select(namespaceName => $"using {namespaceName};"));
        source.PrependLine();
        source.PrependLine("#nullable enable");

        return source;
    }
}