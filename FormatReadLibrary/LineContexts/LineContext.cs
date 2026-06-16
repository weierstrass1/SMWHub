using Validations;

namespace FormatReadLibrary.LineContexts;

public abstract class LineContext
{
    public abstract int LineIndex { get; }
    public abstract string LineContent { get; }
    public abstract string FilePath { get; }
    public static implicit operator ValidationContext(LineContext value)
    {
        return new(value.FilePath, value.LineIndex, value.LineContent);
    }
}
