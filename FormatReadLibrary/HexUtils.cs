using ASMCodeUtils;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace FormatReadLibrary;
public static partial class HexUtils
{
    public static string ValuesToString(int[]? arr, int valueSize, int linebreak = 16)
    {
        if (arr == null || arr.Length == 0)
            return "";
        string prev = valueSize switch
        {
            3 or 4 => "dw",
            5 or 6 => "dl",
            _ => "db",
        };
        StringBuilder sb = new();
        for (int i = 0; i < arr.Length; i++)
        {
            if (i % linebreak == 0)
                sb.Append($"\n\t{prev} ");
            else
                sb.Append(',');

            sb.Append($"${arr[i].ToString($"X{valueSize}")}");
        }
        return sb.ToString();
    }
    public static int[] GetValues(string input)
    {
        string cleanInput = ASMEditUtils.CleanString(input);
        MatchCollection matches = valueRegex().Matches(cleanInput);
        int[] values = new int[matches.Count];
        string currentValue;

        int i = 0;
        foreach (Match match in matches)
        {
            if (match.Value[0] == '$')
            {
                currentValue = match.ToString()![1..];
                values[i] = int.Parse(currentValue, NumberStyles.HexNumber);
            }
            else
            {
                currentValue = match.ToString()!;
                values[i] = int.Parse(currentValue);
            }
            i++;
        }

        return values;
    }

    [GeneratedRegex("(\\$[A-Fa-z0-9]+|[0-9]+)", RegexOptions.Multiline)]
    private static partial Regex valueRegex();
}
