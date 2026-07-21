using FormatReadLibrary.LineContexts;
using System.Globalization;
using System.Numerics;
using Validations;
using ZWXStateMachine;
using ZWXStateMachine.Interfaces;

namespace FormatReadLibrary.Readers;

public partial class NormalSpriteCFGReader
{
    private class NormalSpriteCFGValuesLineStateLogic<TValue>(string lineName, char separator, TValue minLimit, TValue maxLimit, params (string, string)[] variables) : IStateBehaviourEnter where TValue : INumber<TValue>
    {
        public bool ExecuteUpdateRightAfterTransition => false;
        private readonly (string, string)[] _variables = variables;
        private readonly TValue _minLimit = minLimit;
        private readonly TValue _maxLimit = maxLimit;
        private readonly char _separator = separator;
        private readonly string _lineName = lineName;
        public void Enter(StateData state)
        {
            var s = split(state);

            for (int i = 0; i < _variables.Length; i++)
            {
                state.Set(_variables[i].Item1, fromString(state, _variables[i].Item2, s[i]));
            }
        }
        private string[] split(StateData state)
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
        private TValue fromString(StateData state, string description, string value)
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
