using FormatReadLibrary.Readers.Validators;
using StateMachine;

namespace FormatReadLibrary.Readers;

public abstract class ParsingContext
{
    public State State { get; private set; }
    protected List<Validator> _validators { get; private set; }
    public FileEnumeratorWithLog FileEnumerator { get; private set; }
    public ParsingContext(FileEnumeratorWithLog fileEnumerator)
    {
        FileEnumerator = fileEnumerator;
        State = new();
        _validators = [];
    }
    protected void AddValidator(Validator validator)
    {
        _validators.Add(validator);
    }
    public abstract bool ProcessEntry();
    protected bool validate()
    {
        foreach(var validator in _validators)
        {
            if (!validator.Validate(this))
                return false;
        }
        return true;
    }
}
