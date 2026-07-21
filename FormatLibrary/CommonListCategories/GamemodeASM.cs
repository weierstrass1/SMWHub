using FormatLibrary.Interfaces;

namespace FormatLibrary.CommonListCategories;

public class GamemodeASM(string baseDirectory) : ICommonListCategory
{
    public string Title => "Gamemode";
    public string BaseDirectory { get; private set; } = baseDirectory;
}
