using FormatLibrary.Interfaces;

namespace FormatLibrary.CommonListCategories;

public class OverworldSprite(string baseDirectory) : ICommonListCategory
{
    public string Title => "Overworlds";
    public string BaseDirectory { get; private set; } = baseDirectory;
}
