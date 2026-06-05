using FormatReadLibrary.Readers.Enumerators;
using StateMachine;
using System.Text.RegularExpressions;

namespace FormatReadLibrary.Readers.StateVariables;

public class FilelistStateVariable(FileEnumeratorWithLog fileEnumerator, bool allowedVariables, bool allowMultiFiles) : IStateVariable<FilePath[]>
{
    private static readonly Regex filepathRegex = FileRegexContainer.EntryFileRegex();
    public bool CleanOnReset { get; set; } = false;
    public FilePath[]? Value { get; set; }
    object? IStateVariable.Value { get => Value; set => Value = (FilePath[]?)value!; }
    private readonly FileEnumeratorWithLog _fileEnumerator = fileEnumerator;
    private readonly bool _allowedVariables = allowedVariables;
    private readonly bool _allowMultiFiles = allowMultiFiles;
    public FilePath[]? GetFrom(Match match)
    {
        string[] values = match.Groups["filelist"].Success ?
            [..match.Groups["filelist"].Value
                .Split(',')
                .Select(s => s.Trim())] :
            [];
        return Value;
    }
}
public record FilePath(string Path, int[] Values);