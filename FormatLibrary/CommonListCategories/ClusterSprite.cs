using FormatReadLibrary.Interfaces;

namespace FormatLibrary.CommonListCategories;

public class ClusterSprite(string baseDirectory) : ICommonListCategory
{
    public string Title => "Clusters";
    public string BaseDirectory { get; private set; } = baseDirectory;
}
