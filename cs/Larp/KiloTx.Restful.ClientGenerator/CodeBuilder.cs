using System.Text;
using JetBrains.Annotations;
using Microsoft.CodeAnalysis.Text;

namespace KiloTx.Restful.ClientGenerator;

[PublicAPI]
internal class CodeBuilder
{
    private readonly StringBuilder _stringBuilder = new();
    private int _indent;
    
    public static implicit operator SourceText(CodeBuilder codeBuilder) =>
        SourceText.From(codeBuilder._stringBuilder.ToString(), Encoding.UTF8);

    public CodeBuilder IncreaseIndent()
    {
        _indent++;
        return this;
    }

    public CodeBuilder DecreaseIndent()
    {
        _indent--;
        return this;
    }

    public CodeBuilder AppendLine()
    {
        _stringBuilder.AppendLine();
        return this;
    }

    public CodeBuilder AppendLine(string line)
    {
        _stringBuilder.AppendLine(new string('\t', _indent) + line);
        return this;
    }

    public CodeBuilder PrependLine(string line)
    {
        _stringBuilder.Insert(0, Environment.NewLine).Insert(0, line);
        return this;
    }

    public CodeBuilder PrependLine()
    {
        _stringBuilder.Insert(0, Environment.NewLine);
        return this;
    }


    public CodeBuilder AppendLines(IEnumerable<string> lines)
    {
        foreach (var line in lines)
            AppendLine(line.TrimEnd('\r'));
        return this;
    }

    public CodeBuilder PrependLines(IEnumerable<string> lines)
    {
        foreach (var line in lines)
            PrependLine(line.TrimEnd('\r'));
        return this;
    }

    public CodeBuilder OpenBlock() => AppendLine("{").IncreaseIndent();

    public CodeBuilder CloseBlock() => DecreaseIndent().AppendLine("}");
}