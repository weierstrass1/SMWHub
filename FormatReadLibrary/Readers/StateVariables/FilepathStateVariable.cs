using FormatLibrary.Entries;
using SMWHubValidations.StateVariableValidations;
using System.Text.RegularExpressions;
using Validations;
using Validations.Interfaces;
using ZWXStateMachine.Interfaces;

namespace FormatReadLibrary.Readers.StateVariables;

public class FilepathStateVariable : StateValidator, IStateVariable<FilePath>, ISelfValidatedStateVariable
{
    private static readonly Regex _filepathRegex = RegexContainer.EntryFileRegex();
    public FilePath? Value { get; set; }
    public bool CleanOnReset { get; set; } = false;
    object? IStateVariable.Value { get => Value; set => Value = (FilePath?)value; }
    private readonly string _baseDirectory;
    public FilepathStateVariable(string baseDirectory, bool allowedVariables)
    {
        _baseDirectory = baseDirectory;
        StateData.AddVariable<string>("Filepath");
        StateData.AddStateVariable("Parameters", new ParametersStateVariable(allowedVariables: allowedVariables));
        addValidator(new ValidatePathIntegrity(this));
        //addValidator(new ValidateFileExists(this));
    }
    public ValidationResult GetFrom(ValidationContext context, string fileEntry)
    {
        Context = context;
        Match match = _filepathRegex.Match(fileEntry);
        if (!match.Success)
        {
            Value = null;
            return new(Context);
        }
        string filepath = Path.Combine(_baseDirectory, match.Groups["file"].Value)!;
        StateData.Set("Filepath", filepath);

        ValidationResult result = validate();
        var parVars = StateData.GetVariable<ParametersStateVariable>("Parameters");
        result.Merge(parVars.GetFrom(Context, fileEntry));
        Value = new(filepath, parVars.Value!);
        return result;
    }
}
