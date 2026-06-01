using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;

namespace FormatReadLibrary;

public static partial class FileRegexContainer
{
    [StringSyntax(StringSyntaxAttribute.Regex)]
    public const string FILE_PATTERN = @"(?<file>[^\s:\/\\]+(?:[\/\\][^\s:\/\\]+)*\.[A-Za-z0-9]+)";
    [StringSyntax(StringSyntaxAttribute.Regex)]
    public const string VALUES_PATTERN = @"(:\s*(?<var>(\@\d+|[0-9a-fA-F]+)(\s((\@\d+|[0-9a-fA-F]+)))*))?";
    [GeneratedRegex(@"\s+")]
    public static partial Regex SpaceRegex();
    [GeneratedRegex(@";.*")]
    public static partial Regex CommentRegex();
    [GeneratedRegex(@"^(?<id>[a-zA-Z][a-zA-Z0-9]*\d+)(_[a-zA-Z0-9]*)?_Poses?ChunksSizes:$")]
    public static partial Regex DynInfoLegacyRegex();
    [GeneratedRegex(@"^(db (\$[a-fA-F0-9]{2}|[0-9]+)(,(\$[a-fA-F0-9]{2}|[0-9]+))*|dw (\$[a-fA-F0-9]{4}|[0-9]+)(,(\$[a-fA-F0-9]{4}|[0-9]+))*|dl (\$[a-fA-F0-9]{6}|[0-9]+)(,(\$[a-fA-F0-9]{6}|[0-9]+))*)$")]
    public static partial Regex NumberTableRegex();
    [GeneratedRegex(@"^\.Pose(?<start>\d+)(\.\.(?<end>\d+))? (?<tiles>\d+)(?<modifier>(q3|h|q))?$")]
    public static partial Regex DynInfoCurrent();
    [GeneratedRegex(@"^(?<name>[a-zA-Z][a-zA-Z0-9]*)_Pose(?<start>\d+)(?<end>\.\.\d+)?:$")]
    public static partial Regex DrawInfoCurrent();
    [GeneratedRegex(@"^\.NumberOfTiles: (?<tiles>\d+)$")]
    public static partial Regex NumOfTilesRegex();
    [GeneratedRegex(@"^\.(?<directives>[a-zA-Z]+)((?<start>\d+)(\.\.(?<end>\d+))?)?:? (?<values>(\$[a-fA-F0-9]{2}|\d+)(,(\$[a-fA-F0-9]{2}|\d+))*)$")]
    public static partial Regex DirectiveRegex();
    [GeneratedRegex(@"^(?<id>[0-9A-Fa-f]+)\s" + FILE_PATTERN + VALUES_PATTERN + @"$")]
    public static partial Regex ListEntryRegex();
    [GeneratedRegex(@"^(?<r>r|R)?(?<idstart>[a-fA-F0-9]+)(-(?<idend>[a-fA-F0-9]+))?(\s*:\s*(?<actlike>[a-fA-F0-9]+))?\s" + FILE_PATTERN + VALUES_PATTERN + @"$")]
    public static partial Regex GPSListEntryRegex();
}
