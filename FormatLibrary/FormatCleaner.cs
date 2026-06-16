using System.Text.RegularExpressions;

namespace FormatLibrary;
public static partial class FormatCleaner
{
    [GeneratedRegex(@"\s+")]
    public static partial Regex SpaceRegex();
    [GeneratedRegex(@";.*")]
    public static partial Regex CommentRegex();
    public static string CleanString(string str)
    {
        string content = str.Replace("\r\n", "\n");
        content = CommentRegex().Replace(content, "");

        Regex space = SpaceRegex();

        content = string.Join('\n', content
                                .Split('\n')
                                .Select(l => space.Replace(l, " ").Trim()));
        return content;
    }
    public static string CleanFileContent(string path)
    {
        return CleanString(File.ReadAllText(path));
    }
}
