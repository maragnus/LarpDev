using Microsoft.CodeAnalysis;

namespace KiloTx.Restful.ClientGenerator;

[Generator(LanguageNames.CSharp)]
public class FileDownloadSourceGenerator : ISourceGenerator
{
    public void Initialize(GeneratorInitializationContext context)
    {
    }

    public void Execute(GeneratorExecutionContext context)
    {
        var source = new SourceCodeBuilder()
            .AppendLine("#nullable enable")
            .AppendLine()
            .AppendLine("namespace KiloTx.Restful;")
            .AppendLine()
            .AppendLine("internal class DownloadFileInfo : Microsoft.Extensions.FileProviders.IFileInfo")
            .OpenBlock()
            .AppendLine("private readonly MemoryStream _stream;")
            .AppendLine("public DownloadFileInfo(MemoryStream stream, string name, int length)")
            .OpenBlock()
            .AppendLine("Name = name;")
            .AppendLine("Length = length;")
            .AppendLine("_stream = stream;")
            .CloseBlock()
            .AppendLine("public Stream CreateReadStream() => _stream;")
            .AppendLine("public bool Exists => true;")
            .AppendLine("public long Length { get; }")
            .AppendLine("public string? PhysicalPath => null;")
            .AppendLine("public string Name { get; }")
            .AppendLine("public DateTimeOffset LastModified => DateTimeOffset.Now;")
            .AppendLine("public bool IsDirectory => false;")
            .CloseBlock();

        context.AddSource("DownloadFileInfo.g.cs", source);
    }
}