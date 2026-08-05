namespace SMWHubPluginAPI.PackagesTypes;

public static class DirectoryExtension
{
    public static bool IsSubFolder(string parent, string child)
    {
        string relativePath = Path.GetRelativePath(parent, child);

        return relativePath != "." &&
               !relativePath.StartsWith(".." + Path.DirectorySeparatorChar) &&
               relativePath != "..";
    }
}
