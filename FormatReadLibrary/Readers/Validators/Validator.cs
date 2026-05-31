using StateMachine;
namespace FormatReadLibrary.Readers.Validators
{
    public abstract class Validator
    {
        protected abstract  (string, Type)[] _variableNames { get; }
        public abstract bool Validate(ParsingContext ctx);
        public Validator(ParsingContext ctx)
        {
            State state = ctx.State;
            foreach (var variable in _variableNames)
            {
                if (!state.HasVariable(variable.Item1))
                    throw new KeyNotFoundException($"Missing {variable.Item1} of type {getFriendlyName(variable.Item2)}.");
            }
        }
        private static string getFriendlyName(Type type)
        {
            if (!type.IsGenericType)
                return type.Name;

            string name = type.Name;
            int index = name.IndexOf('`');

            if (index >= 0)
                name = name[..index];

            string[] args = [.. type
                .GetGenericArguments()
                .Select(getFriendlyName)];

            return $"{name}<{string.Join(", ", args)}>";
        }
    }
}
