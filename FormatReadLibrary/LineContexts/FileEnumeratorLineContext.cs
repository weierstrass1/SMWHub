using SMWHubEnumerators;

namespace FormatReadLibrary.LineContexts;

public sealed class FileEnumeratorLineContext(FileEnumerator fileEnumerator) : LineContext
{
    public bool IsLastLine => _fileEnumerator.IsLastLine;
    public override int LineIndex => _fileEnumerator.LineIndex;
    public override string LineContent => _fileEnumerator.Current;
    public override string FilePath => _fileEnumerator.FilePath;
    private readonly FileEnumerator _fileEnumerator = fileEnumerator;
    public static explicit operator FileEnumeratorLineContext(FileEnumerator fileEnumerator)
    {
        return new(fileEnumerator);
    }
    public static implicit operator FileEnumerator(FileEnumeratorLineContext context)
    {
        return context._fileEnumerator;
    }
    public bool MoveNext() => _fileEnumerator.MoveNext();
}
