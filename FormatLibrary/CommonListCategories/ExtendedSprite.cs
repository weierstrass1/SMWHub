using FormatLibrary.Interfaces;

namespace FormatLibrary.CommonListCategories;

public class ExtendedSprite(string baseDirectory) : ICommonListCategory
{
    public string Title => "Extendeds";
    public string BaseDirectory { get; private set; } = baseDirectory;
}
