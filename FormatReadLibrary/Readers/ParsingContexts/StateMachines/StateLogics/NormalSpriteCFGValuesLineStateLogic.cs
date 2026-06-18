using FormatReadLibrary.LineContexts;
using StateMachine;
using StateMachine.Interfaces;
using System.Globalization;
using System.Numerics;
using System.Reflection;
using Validations;

namespace FormatReadLibrary.Readers;

public partial class NormalSpriteCFGReader
{
    private class NormalSpriteCFGValuesLineStateLogic<TValue>(NSCFGStateEnum id, string lineName, char separator, TValue minLimit, TValue maxLimit, params (string, string)[] variables) : IStateLogicStart<NSCFGStateEnum> where TValue : INumber<TValue>
    {
        public bool ExecuteLoopRightAfterTransition => false;
        public NSCFGStateEnum ID => _id;
        private readonly (string, string)[] _variables = variables;
        private readonly TValue _minLimit = minLimit;
        private readonly TValue _maxLimit = maxLimit;
        private readonly char _separator = separator;
        private readonly string _lineName = lineName;
        private readonly NSCFGStateEnum _id = id;
        public void Start(State state)
        {
            var s = split(state);

            for (int i = 0; i < _variables.Length; i++)
            {
                state.Set(_variables[i].Item1, fromString(state, _variables[i].Item2, s[i]));
            }
        }
        private string[] split(State state)
        {
            string[] split = [.. state.Get<LineContext>("Context")!.LineContent
                    .Split(_separator)
                    .Where(s => !string.IsNullOrWhiteSpace(s))];
            if (split.Length != _variables.Length)
            {
                state.Get<ValidationResult>("Validation")!.AddError("");
                split = ["0", "0"];
            }
            return split;
        }
        private TValue fromString(State state, string description, string value)
        {
            if (!TValue.TryParse(value, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out TValue? result))
                state.Get<ValidationResult>("Validation")!.AddError("");
            result ??= TValue.Zero;
            if (result < _minLimit || result > _maxLimit)
                state.Get<ValidationResult>("Validation")!.AddError("");
            return result;
        }
    }
}
