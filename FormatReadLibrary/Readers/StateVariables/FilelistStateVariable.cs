using StateMachine;
using System.Text.RegularExpressions;

namespace FormatReadLibrary.Readers.StateVariables;

public class FilelistStateVariable : StateValidator, IStateVariable<FilePath[]>, ISelfValidatedStateVariable
{
    private static readonly Regex _fileListRegex = RegexContainer.FileListRegex();
    public bool CleanOnReset { get; set; } = false;
    public FilePath[]? Value 
    { 
        get => State.Get<FilePath[]>("Filelist"); 
        set => State.Set("Filelist", value); 
    }
    object? IStateVariable.Value { get => Value; set => Value = (FilePath[]?)value!; }
    private readonly bool _allowedVariables;
    private readonly string _baseDirectory;
    public FilelistStateVariable(string baseDirectory, bool allowedVariables, bool allowedMultiline)
    {
        _allowedVariables = allowedVariables;
        _baseDirectory = baseDirectory;
        State.AddVariable("Filelist", new StateVariable<FilePath[]>());
        addValidator(new ValidateFileListAmount(this, allowedMultiline));
    }
    public ValidationResult GetFrom(string entry)
    {
        Match match = _fileListRegex.Match(entry);
        if (!match.Success)
        {
            Value = [];
            return new();
        }
        ValidationResult result = new();
        string[] values = match.Groups["filelist"].Success ?
            [..match.Groups["filelist"].Value
                .Split(',')
                .Select(s => s.Trim())] :
            [];
        List<FilePath?> fpaths = [];

        FilepathStateVariable fpathStateVariable = new(_baseDirectory, _allowedVariables);
        foreach(var value in values)
        {
            result.Merge(fpathStateVariable.GetFrom(value));
            fpaths.Add(fpathStateVariable.Value);
        }
        Value = [.. fpaths.Where(fp => fp != null).Select(fp => fp!)];

        result.Merge(validate());
        if(!result.IsValid)
            Value = [];

        return result;
    }
}
