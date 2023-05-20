using System.Text;
using JetBrains.Annotations;
using Microsoft.CodeAnalysis.Text;

namespace KiloTx.Restful.ClientGenerator;

[PublicAPI]
internal class SourceCodeBuilder
{
    private readonly StringBuilder _stringBuilder = new();
    private int _indent;

    public static implicit operator SourceText(SourceCodeBuilder sourceCodeBuilder) =>
        SourceText.From(sourceCodeBuilder._stringBuilder.ToString(), Encoding.UTF8);

    public SourceCodeBuilder IncreaseIndent()
    {
        _indent++;
        return this;
    }

    public SourceCodeBuilder DecreaseIndent()
    {
        _indent--;
        return this;
    }

    public SourceCodeBuilder AppendLine()
    {
        _stringBuilder.AppendLine();
        return this;
    }

    public SourceCodeBuilder AppendLine(string line)
    {
        _stringBuilder.AppendLine(new string('\t', _indent) + line);
        return this;
    }

    public SourceCodeBuilder PrependLine(string line)
    {
        _stringBuilder.Insert(0, "\n").Insert(0, line);
        return this;
    }

    public SourceCodeBuilder PrependLine()
    {
        _stringBuilder.Insert(0, "\n");
        return this;
    }

    public SourceCodeBuilder AppendLines(IEnumerable<string> lines)
    {
        foreach (var line in lines)
            AppendLine(line);
        return this;
    }

    public SourceCodeBuilder PrependLines(IEnumerable<string> lines)
    {
        foreach (var line in lines)
            PrependLine(line);
        return this;
    }

    public SourceCodeBuilder OpenBlock() => AppendLine("{").IncreaseIndent();

    public SourceCodeBuilder CloseBlock(string? suffix = null) => suffix == null
        ? DecreaseIndent().AppendLine("}")
        : DecreaseIndent().AppendLine($"}}{suffix}");
}