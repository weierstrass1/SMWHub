using FormatLibrary.Interfaces;

namespace SMWHubSprites.CommonListCategories;

public class ClusterSprite(string baseDirectory) : ICommonListCategory
{
    public string Title { get; } = "Clusters";
    public string BaseDirectory { get; private set; } = baseDirectory;
}
