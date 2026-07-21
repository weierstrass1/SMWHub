using FormatLibrary.Interfaces;

namespace FormatLibrary.CommonListCategories;

public class OverworldASM(string baseDirectory) : ICommonListCategory
{
    public string Title => "Overworld";
    public string BaseDirectory { get; private set; } = baseDirectory;
}
