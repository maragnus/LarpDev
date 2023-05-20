using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace KiloTx.Restful.ClientGenerator;

internal static class Helpers
{
    internal static string ToCamelCase(string value) =>
        value.Length == 0
            ? value
            : char.ToLowerInvariant(value[0]) + value[1..];

    public static ITypeSymbol? GetTypeFromAttribute(GeneratorSyntaxContext context,
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

    internal static IEnumerable<string> GetUsings(ISymbol? symbol)
    {
        if (symbol?.ContainingNamespace == null) yield break;

        yield return symbol.ContainingNamespace.ToDisplayString();

        if (symbol is not INamedTypeSymbol { IsGenericType: true } genericType) yield break;

        foreach (var namespaceName in genericType.TypeArguments.SelectMany(GetUsings))
            yield return namespaceName;
    }

    internal static ImmutableArray<InterfaceAttribute> GetInterfaces(ISymbol type)
    {
        return type.GetAttributes()
            .Where(x => x.AttributeClass?.Name == nameof(RestfulImplementationAttribute))
            .Select(x => new InterfaceAttribute(x.AttributeClass!.TypeArguments[0], x.AttributeClass!.TypeArguments[1]))
            .ToImmutableArray();
    }

    internal record InterfaceAttribute(ITypeSymbol Interface, ITypeSymbol HttpClientFactory);
}