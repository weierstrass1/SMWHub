using FormatLibrary.Interfaces;

namespace SMWHubSprites.CommonListCategories;

public class NormalSprite(string baseDirectory) : ICommonListCategory
{
    public string Title { get; } = "Sprites";
    public string BaseDirectory { get; private set; } = baseDirectory;
}
