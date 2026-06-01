using FormatReadLibrary.Logging.Categories;
using LogRegister;

namespace FormatReadLibrary.Logging.Wrappers;

public static class ConsoleWrapper
{
    public static void RenderAction(string text, ILogCategory category, SpanType type, bool mustWrite = false)
    {
        if (!mustWrite)
            return;
        if (type == SpanType.Prefix)
            return;
        var color = category switch
        {
            Title => type == SpanType.NormalText ?
                                ConsoleColor.Magenta :
                                ConsoleColor.DarkMagenta,
            Error => type == SpanType.NormalText ?
                                ConsoleColor.Red :
                                ConsoleColor.DarkRed,
            Warning => type == SpanType.NormalText ?
                                ConsoleColor.Yellow :
                                ConsoleColor.DarkYellow,
            Success => type == SpanType.NormalText ?
                                ConsoleColor.Green :
                                ConsoleColor.DarkGreen,
            _ => type == SpanType.NormalText ?
                                ConsoleColor.Gray :
                                ConsoleColor.Cyan,
        };
        Console.ForegroundColor = color;
        Console.Write(text);
        Console.ResetColor();
    }
}
