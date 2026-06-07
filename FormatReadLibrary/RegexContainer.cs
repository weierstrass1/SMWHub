using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;

namespace FormatReadLibrary;

public static partial class RegexContainer
{
    [StringSyntax(StringSyntaxAttribute.Regex)]
    public const string DEC_VALUE_REGEX = @"\d+";
    [StringSyntax(StringSyntaxAttribute.Regex)]
    public const string HEX_VALUE_REGEX = @"[a-fA-F\d]+";
    [StringSyntax(StringSyntaxAttribute.Regex)]
    public const string RANGE_PATTERN = $"""(?<ids>(?<multiple>\[\s*({DEC_VALUE_REGEX}(\.\.{DEC_VALUE_REGEX})?)(\s*,\s*({DEC_VALUE_REGEX}(\.\.{DEC_VALUE_REGEX})?))*\s*\])|(?<range>(?<start>{DEC_VALUE_REGEX})\.\.(?<end>{DEC_VALUE_REGEX}))|(?<single>{DEC_VALUE_REGEX}))""";
    [StringSyntax(StringSyntaxAttribute.Regex)]
    public const string HEX_RANGE_PATTERN = $"""(?<ids>(?<multiple>\[\s*({HEX_VALUE_REGEX}(\.\.{HEX_VALUE_REGEX})?)(\s*,\s*({HEX_VALUE_REGEX}(\.\.{HEX_VALUE_REGEX})?))*\s*\])|(?<range>(?<start>{HEX_VALUE_REGEX})\.\.(?<end>{HEX_VALUE_REGEX}))|(?<single>{HEX_VALUE_REGEX}))""";
    [StringSyntax(StringSyntaxAttribute.Regex)]
    public const string FILE_LIST_PATTERN = @"(?<filelist>([^\s:\/\\]+(?:[\/\\][^\s:\/\\]+)*\.[A-Za-z0-9]+)(:\s*(\@\d+|[0-9a-fA-F]+)(\s((\@\d+|[0-9a-fA-F]+)))*)?(\s*,\s*([^\s:\/\\]+(?:[\/\\][^\s:\/\\]+)*\.[A-Za-z0-9]+)(:\s*(\@\d+|[0-9a-fA-F]+)(\s((\@\d+|[0-9a-fA-F]+)))*)?)*)";
    [StringSyntax(StringSyntaxAttribute.Regex)]
    public const string FILE_PATTERN = @"(?<file>[^\s:\/\\]+(?:[\/\\][^\s:\/\\]+)*\.[A-Za-z0-9]+)";
    [StringSyntax(StringSyntaxAttribute.Regex)]
    public const string VALUES_PATTERN = @"(:\s*(?<var>(\@\d+|[0-9a-fA-F]+)(\s((\@\d+|[0-9a-fA-F]+)))*))?";
    [GeneratedRegex(FILE_LIST_PATTERN)]
    public static partial Regex FileListRegex();
    [GeneratedRegex(FILE_PATTERN)]
    public static partial Regex FileRegex();
    [GeneratedRegex($"{FILE_PATTERN}{VALUES_PATTERN}")]
    public static partial Regex EntryFileRegex();
    [GeneratedRegex(VALUES_PATTERN)]
    public static partial Regex ValuesRegex();
    [GeneratedRegex(@"^(?<id>[a-zA-Z][a-zA-Z0-9]*\d+)(_[a-zA-Z0-9]*)?_Poses?ChunksSizes:$")]
    public static partial Regex DynInfoLegacyRegex();
    [GeneratedRegex(@"^(db (\$[a-fA-F0-9]{2}|[0-9]+)(,(\$[a-fA-F0-9]{2}|[0-9]+))*|dw (\$[a-fA-F0-9]{4}|[0-9]+)(,(\$[a-fA-F0-9]{4}|[0-9]+))*|dl (\$[a-fA-F0-9]{6}|[0-9]+)(,(\$[a-fA-F0-9]{6}|[0-9]+))*)$")]
    public static partial Regex NumberTableRegex();
    [GeneratedRegex($"""^\.Pose{RANGE_PATTERN}:?\s(?<tiles>\d+)(?<modifier>(q3|h|q))?$""")]
    public static partial Regex DynInfoCurrentRegex();
    [GeneratedRegex($"""^(?<name>[a-zA-Z][a-zA-Z0-9]*)_Pose{RANGE_PATTERN}:$""")]
    public static partial Regex DrawInfoCurrent();
    [GeneratedRegex(@"^\.NumberOfTiles: (?<tiles>\d+)$")]
    public static partial Regex NumOfTilesRegex();
    [GeneratedRegex(@"^\.(?<directives>[a-zA-Z]+)((?<start>\d+)(\.\.(?<end>\d+))?)?:? (?<values>(\$[a-fA-F0-9]{2}|\d+)(,(\$[a-fA-F0-9]{2}|\d+))*)$")]
    public static partial Regex DirectiveRegex();
    [GeneratedRegex($"""^{HEX_RANGE_PATTERN}\s*{FILE_PATTERN}{VALUES_PATTERN}$""")]
    public static partial Regex ListEntryRegex();
    [GeneratedRegex($"""^(?<r>r|R)?(?<idstart>[a-fA-F0-9]+)(-(?<idend>[a-fA-F0-9]+))?(\s*:\s*(?<actlike>[a-fA-F0-9]+))?\s{FILE_PATTERN}{VALUES_PATTERN}$""")]
    public static partial Regex GPSListEntryRegex();
    [GeneratedRegex(@"([a-z])([A-Z])")]
    private static partial Regex LowerUpperRegex();

    public static string ToUpperSpacedName(this Type type)
    {
        string name = type.Name;

        int genericIndex = name.IndexOf('`');
        if (genericIndex >= 0)
            name = name[..genericIndex];

        name = LowerUpperRegex().Replace(name, "$1 $2");

        return name.ToUpperInvariant();
    }
}
