using SMWHubEnumerators;

namespace FormatReadLibrary.LineContexts;

public sealed class FileEnumeratorLineContext(FileLineEnumerator fileEnumerator) : LineContext
{
    public bool IsLastLine => _fileEnumerator.IsLastLine;
    public override int LineIndex => _fileEnumerator.LineIndex;
    public override string LineContent => _fileEnumerator.Current;
    public override string FilePath => _fileEnumerator.FilePath;
    private readonly FileLineEnumerator _fileEnumerator = fileEnumerator;
    public static explicit operator FileEnumeratorLineContext(FileLineEnumerator fileEnumerator)
    {
        return new(fileEnumerator);
    }
    public static implicit operator FileLineEnumerator(FileEnumeratorLineContext context)
    {
        return context._fileEnumerator;
    }
    public bool MoveNext() => _fileEnumerator.MoveNext();
}
