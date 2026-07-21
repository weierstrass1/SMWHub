using FormatLibrary.Interfaces;

namespace FormatLibrary.CommonListCategories;

public class NormalSprite(string baseDirectory) : ICommonListCategory
{
    public string Title => "Sprites";
    public string BaseDirectory { get; private set; } = baseDirectory;
}
