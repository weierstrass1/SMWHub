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
    private class NormalSpriteCFGValuesLineStateLogic<TValue> : IStateLogicStart<NSCFGStateEnum> where TValue : INumber<TValue>
    {
        public bool ExecuteLoopRightAfterTransition => false;
        public NSCFGStateEnum ID => _id;
        private readonly (Type, string, string)[] _variables;
        private readonly TValue _minLimit;
        private readonly TValue _maxLimit;
        private readonly char _separator;
        private readonly string _lineName;
        private readonly NSCFGStateEnum _id;
        private readonly MethodInfo _stateSet = typeof(State).GetMethod(nameof(State.Set))!;
        private readonly bool _useGenericType = true;
        public NormalSpriteCFGValuesLineStateLogic(NSCFGStateEnum id, string lineName, char separator, TValue minLimit, TValue maxLimit, params (string, string)[] variables)
        {
            _id = id;
            _variables = [.. variables.Select(v => (typeof(TValue), v.Item1, v.Item2))];
            _minLimit = minLimit;
            _maxLimit = maxLimit;
            _separator = separator;
            _lineName = lineName;
        }
        public NormalSpriteCFGValuesLineStateLogic(NSCFGStateEnum id, string lineName, char separator, TValue minLimit, TValue maxLimit, params (Type, string, string)[] variables)
        {
            _id = id;
            _variables = variables;
            _minLimit = minLimit;
            _maxLimit = maxLimit;
            _separator = separator;
            _lineName = lineName;
            _useGenericType = false;
        }
        public void Start(State state)
        {
            var s = split(state);

            if (_useGenericType)
                startWithGenericType(state, s);
            else
                startWithMultiTypes(state, s);
        }
        private void startWithGenericType(State state, string[] s)
        {
            for (int i = 0; i < _variables.Length; i++)
            {
                state.Set(_variables[i].Item2, fromString(state, _variables[i].Item3, s[i]));
            }
        }
        private void startWithMultiTypes(State state, string[] s)
        {
            Dictionary<Type, MethodInfo> generics = [];
            Type t = typeof(TValue);

            for (int i = 0; i < _variables.Length; i++)
            {
                if (!_variables[i].Item1.IsAssignableFrom(t))
                    throw new InvalidCastException();
                if (!generics.TryGetValue(_variables[i].Item1, out MethodInfo? generic))
                {
                    generic = _stateSet.MakeGenericMethod(_variables[i].Item1);
                    generics.Add(_variables[i].Item1, generic);
                }
                generic!.Invoke(state, [_variables[i].Item2, fromString(state, _variables[i].Item3, s[i])]);
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
