using FormatReadLibrary.Interfaces;

namespace FormatLibrary.CommonListCategories;

public class LevelASM(string baseDirectory) : ICommonListCategory
{
    public string Title => "Level";
    public string BaseDirectory { get; private set; } = baseDirectory;
}
