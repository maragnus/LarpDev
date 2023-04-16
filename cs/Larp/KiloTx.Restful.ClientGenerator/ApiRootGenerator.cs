using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace KiloTx.Restful.ClientGenerator;

[Generator(LanguageNames.CSharp)]
public class RestfulSourceGenerator : IIncrementalGenerator
{
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

        var usings = new HashSet<string>() { "System.Text.Json", "System.Net.Http.Json" };
        usings.AddRange(interfaces.Select(interfaceType => interfaceType.ContainingNamespace.ToDisplayString()));

        var source = new CodeBuilder();
        source.AppendLine();
        source.AppendLine($"namespace {type.ContainingNamespace};");
        source.AppendLine();
        source.AppendLine($"partial class {type.Name} : {implements}");
        source.OpenBlock();
        
        source.AppendLine("private HttpClient _httpClient;");
        source.AppendLine();
        source.AppendLine($"protected {type.Name}(HttpClient httpClient)");
        source.OpenBlock();
        source.AppendLine("_httpClient = httpClient;");
        source.CloseBlock();
        source.AppendLine();

        foreach (var implementType in interfaces)
        {
            var members = implementType.GetMembers()
                .Select(x => x as IMethodSymbol).Where(x => x != null);
            foreach (var member in members)
            {
                if (member == null) continue;

                try
                {
                    // using Return types
                    usings.AddRange(GetUsings(member.ReturnType));

                    var taskType = member.ReturnType as INamedTypeSymbol ??
                                   throw new InvalidOperationException($"Method {member.Name} must have a Task return type");
                    // if (taskType.ToDisplayString() != "System.Threading.Task")
                    //     throw new InvalidOperationException($"Method {member.Name} returns {taskType.ToDisplayString()} but must be async Task<>");
                    var returnType = taskType.IsGenericType ? taskType.TypeArguments.Single() : null;
                    
                    foreach (var argument in member.Parameters)
                    {
                        usings.AddRange(GetUsings(argument.Type));
                    }

                    var arguments = string.Join(", ", member.Parameters
                        .Select(x => x.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat)));
                    
                    var argumentsNames = string.Join(", ", member.Parameters
                        .Select(x => x.ToDisplayParts(SymbolDisplayFormat.MinimallyQualifiedFormat).Last()));

                    source.AppendLine($@"async {member.ReturnType.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat)} {implementType.Name}.{member.Name}({arguments})");
                    source.OpenBlock();

                    var url = "/api/blah";
                    var data = @", new StringContent("""")";
                    
                    // GET uses query string
                    // POST uses json
                    
                    if (argumentsNames.Length > 0)
                    {
                        source.AppendLine($"var httpContent = new {{ {argumentsNames} }};");
                        data = ", JsonContent.Create(httpContent)";
                    }
                    
                    if (returnType != null)
                    {
                        source
                            .AppendLine(@$"var response = await _httpClient.PostAsync(""{url}""{data});")
                            .AppendLine(@"await using var stream = await response.Content.ReadAsStreamAsync();")
                            .AppendLine(@$"return await JsonSerializer.DeserializeAsync<{returnType.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat)}>(stream) ?? throw new BadRequestException();");
                    }
                    else
                    {
                        source.AppendLine(@$"await _httpClient.PostAsync(""{url}""{data});");
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

        source.PrependLine();
        source.PrependLines(usings.Order().Select(namespaceName => $"using {namespaceName};"));
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