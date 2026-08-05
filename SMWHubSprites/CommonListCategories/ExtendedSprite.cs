using FormatLibrary.Interfaces;

namespace SMWHubSprites.CommonListCategories;

public class ExtendedSprite(string baseDirectory) : ICommonListCategory
{
    public string Title { get; } = "Extendeds";
    public string BaseDirectory { get; private set; } = baseDirectory;
}
