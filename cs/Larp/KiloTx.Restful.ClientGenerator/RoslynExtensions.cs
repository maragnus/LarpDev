using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace KiloTx.Restful;

public static class RoslynExtensions
{
    // public static T GetAttributeValue<T>(this AttributeData attributeData, string key, T defaultValue = default)
    // {
    //     var attributeRecord = attributeData.NamedArguments
    //         .FirstOrDefault(c => c.Key == key)
    //         .Value;
    //
    //     return attributeRecord is { IsNull: false } ? (T)attributeRecord.Value.Value : defaultValue;
    // }

    public static bool HasAttribute<TAttribute>(this ClassDeclarationSyntax classDeclaration)
    {
        var attributeName = typeof(TAttribute).Name;
        return classDeclaration.AttributeLists
            .SelectMany(attributeList => attributeList.Attributes)
            .Any(attribute => attribute.Name.NormalizeWhitespace().ToFullString() == attributeName);
    }

}