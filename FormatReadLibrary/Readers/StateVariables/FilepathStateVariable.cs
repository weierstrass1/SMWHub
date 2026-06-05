using FormatReadLibrary.Logging.Enumerators;
using FormatReadLibrary.Readers.Validators;
using StateMachine;
using System.Text.RegularExpressions;

namespace FormatReadLibrary.Readers.StateVariables;

public class FilepathStateVariable : StateValidator, IStateVariable<string>
{
    public string? Value { get; set; }
    public bool CleanOnReset { get; set; } = false;
    object? IStateVariable.Value { get => Value; set => Value = (string?)value; }
    private readonly ValidatePathIntegrity _validatePathIntegrity;
    private readonly ValidateFileExists _validateFileExists;
    private readonly ValidateEntryParameters _validateEntryVariables;
    public FilepathStateVariable(FileEnumeratorWithLog fileEnumerator, bool allowedVariables)
    {
        _validatePathIntegrity = new(fileEnumerator);
        _validateFileExists = new(fileEnumerator.Log);
        _validateEntryVariables = new(fileEnumerator, allowedVariables);
    }
    public string? GetFrom(Match match, string basedirectory = "")
    {
        Value = match.Groups["file"].Success ?
            Path.Combine(basedirectory, match.Groups["file"].Value) :
            null;
        return Value;
    }
    public bool Validate()
    {

    }
}
