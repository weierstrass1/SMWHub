using System.Text.RegularExpressions;

namespace FormatReadLibrary;

public static class FileUtils
{
    public static string CleanString(string str)
    {
        string content = FileRegexContainer.CommentRegex().Replace(str, "");

        Regex space = FileRegexContainer.SpaceRegex();

        content = string.Join('\n', content
                                .Split('\n')
                                .Select(l => space.Replace(l, " ").Trim()));
        return content;
    }
    public static string CleanFileContent(string path)
    {
        string content = File.ReadAllText(path).Replace("\r\n", "\n");
        return CleanString(content);
    }
}
