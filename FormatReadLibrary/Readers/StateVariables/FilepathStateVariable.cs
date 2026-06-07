using StateMachine;
using System.Text.RegularExpressions;

namespace FormatReadLibrary.Readers.StateVariables;

public class FilepathStateVariable : StateValidator, IStateVariable<FilePath>, ISelfValidatedStateVariable
{
    private static readonly Regex filepathRegex = RegexContainer.EntryFileRegex();
    public FilePath? Value { get; set; }
    public bool CleanOnReset { get; set; } = false;
    object? IStateVariable.Value { get => Value; set => Value = (FilePath?)value; }
    private readonly string _baseDirectory;
    public FilepathStateVariable(string baseDirectory, bool allowedVariables)
    {
        _baseDirectory = baseDirectory;
        State.AddVariable("Filepath", new StateVariable<string>());
        State.AddVariable("Parameters", new ParametersStateVariable());
        addValidator(new ValidatePathIntegrity(this));
        //addValidator(new ValidateFileExists(this));
    }
    public ValidationResult GetFrom(string fileEntry)
    {
        Match match = filepathRegex.Match(fileEntry);
        if(!match.Success)
        {
            Value = null;
            return new();
        }
        string filepath = Path.Combine(_baseDirectory, match.Groups["file"].Value)!;
        State.Set("Filepath", filepath);

        ValidationResult result = validate();
        var parVars = State.GetVariable<ParametersStateVariable>("Parameters");
        result.Merge(parVars.GetFrom(fileEntry));
        Value = new(filepath, parVars.Value!);
        return result;
    }
    public ValidationResult validate(FileEnumeratorWithLog fileEnumerator)
    {
        ValidationResult result = base.validate();
        if (!result)
            ValidatorLogAdapter.LogValidatorResult(fileEnumerator, result);
        return result;
    }
}
public record FilePath(string Path, int[] Values);
