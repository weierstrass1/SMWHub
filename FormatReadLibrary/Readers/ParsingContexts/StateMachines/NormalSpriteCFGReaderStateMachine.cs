using FormatLibrary.Entries;
using FormatReadLibrary.LineContexts;
using StateMachine;
using StateMachine.Interfaces;
using StateMachine.StateLogics;
using System.Globalization;
using System.Text.RegularExpressions;
using Validations;

namespace FormatReadLibrary.Readers.ParsingContexts.StateMachines;

public static partial class NormalSpriteCFGReader
{
    private enum NSCFGStateEnum
    {
        Type,
        ActLike,
        Tweaks,
        Props,
        File,
        ExBytes,
        Done
    }
    private partial class NormalSpriteCFGReaderStateMachine : StateMachine<NSCFGStateEnum>
    {
        public NormalSpriteCFGReaderStateMachine(State state) : base(state, NSCFGStateEnum.Type, configTransitions(), configStates(), new StateLogic<NSCFGStateEnum>(NSCFGStateEnum.Done, false))
        {
            state.AddVariable("PropsDone", false);
        }
        private static Dictionary<NSCFGStateEnum, IStateLogic<NSCFGStateEnum>> configStates()
        {
            Dictionary<NSCFGStateEnum, IStateLogic<NSCFGStateEnum>> states = [];
            states.Add(NSCFGStateEnum.Type, 
                new DelegateStateLogicStart<NSCFGStateEnum>(NSCFGStateEnum.Type, state =>
            {
                state.Set("Type", fromByte(state, "Sprite Type", state.Get<LineContext>("context")!.LineContent, 0, 2));
            }));
            states.Add(NSCFGStateEnum.ActLike,
                new DelegateStateLogicStart<NSCFGStateEnum>(NSCFGStateEnum.Type, state =>
            {
                state.Set("ActLike", fromByte(state, "Act Like", state.Get<LineContext>("context")!.LineContent));
            }));
            states.Add(NSCFGStateEnum.Tweaks,
                new DelegateStateLogicStart<NSCFGStateEnum>(NSCFGStateEnum.Type, state =>
            {
                var s = split(state, "Tweaks Line", 6);

                state.Set<Tweak1656>("$1656", fromByte(state, "Tweak 1656", s[0]));
                state.Set<Tweak1662>("$1662", fromByte(state, "Tweak 1662", s[1]));
                state.Set<Tweak166E>("$166E", fromByte(state, "Tweak 166E", s[2]));
                state.Set<Tweak167A>("$167A", fromByte(state, "Tweak 167A", s[3]));
                state.Set<Tweak1686>("$1686", fromByte(state, "Tweak 1686", s[4]));
                state.Set<Tweak190F>("$190F", fromByte(state, "Tweak 190F", s[5]));
            }));
            states.Add(NSCFGStateEnum.Props,
                new DelegateStateLogicStart<NSCFGStateEnum>(NSCFGStateEnum.Type, state =>
            {
                state.Set("PropsDone", true);
                var s = split(state, "Property Line", 2);
                state.Set<Tweak1656>("Prop1", fromByte(state, "Extra Property 1", s[0]));
                state.Set<Tweak1662>("Prop2", fromByte(state, "Extra Property 2", s[1]));
            }));
            states.Add(NSCFGStateEnum.File,
                new DelegateStateLogicStart<NSCFGStateEnum>(NSCFGStateEnum.Type, state =>
            {
                if(state.Get<int>("Type") == 0)
                    state.Get<ValidationResult>("validation")!.AddError("");
                state.Set("FilePath", state.Get<LineContext>("context")!.LineContent);
            }));
            states.Add(NSCFGStateEnum.ExBytes,
                new DelegateStateLogicStart<NSCFGStateEnum>(NSCFGStateEnum.Type, state =>
            {
                if (state.Get<int>("Type") == 0)
                    state.Get<ValidationResult>("validation")!.AddError("");
                var s = split(state, "Extra Byte Line", 2);
                state.Set("CleanEBAmount", fromByte(state, "Extra Byte Amount when Extra Bit is Clear", s[0], 0, 12));
                state.Set("SetEBAmount", fromByte(state, "Extra Byte Amount when Extra Bit is Set", s[1], 0, 12));
            }));
            return states;
        }
        private static Dictionary<NSCFGStateEnum, List<StateEnumTransitionPair<NSCFGStateEnum>>> configTransitions()
        {
            ITransition alwaysTransition = new DelegateTransition(state => true);
            ITransition filenameTransition = new DelegateTransition(state =>
            {
                Regex r = exclusivePathCharacters();
                return r.IsMatch(state.Get<LineContext>("context")!.LineContent);
            });
            ITransition exByteTransition = new DelegateTransition(state =>
            {
                return state.Get<LineContext>("context")!.LineContent.Contains(':');
            });
            ITransition propsTransition = new DelegateTransition(state =>
            {
                return state.Get<bool>("PropsDone");
            });
            Dictionary<NSCFGStateEnum, List<StateEnumTransitionPair<NSCFGStateEnum>>> transitions = [];
            transitions.Add(NSCFGStateEnum.Type, [new(NSCFGStateEnum.ActLike, alwaysTransition)]);
            transitions.Add(NSCFGStateEnum.ActLike, [new(NSCFGStateEnum.Tweaks, alwaysTransition)]);
            transitions.Add(NSCFGStateEnum.Tweaks, [
                new(NSCFGStateEnum.File, filenameTransition),
                new(NSCFGStateEnum.ExBytes, exByteTransition),
                new(NSCFGStateEnum.Props, alwaysTransition)
                ]);
            transitions.Add(NSCFGStateEnum.Props, [
                new(NSCFGStateEnum.File, filenameTransition),
                new(NSCFGStateEnum.ExBytes, exByteTransition),
                new(NSCFGStateEnum.Done, alwaysTransition)
            ]);
            transitions.Add(NSCFGStateEnum.File, [
                new(NSCFGStateEnum.ExBytes, exByteTransition),
                new(NSCFGStateEnum.Props, propsTransition),
                new(NSCFGStateEnum.Done, alwaysTransition)
            ]);
            transitions.Add(NSCFGStateEnum.ExBytes, [
                new(NSCFGStateEnum.File, exByteTransition),
                new(NSCFGStateEnum.Props, propsTransition),
                new(NSCFGStateEnum.Done, alwaysTransition)
            ]);
            return transitions;
        }
        private static string[] split(State state, string stateName, int amountOfValues, char separator = ' ')
        {
            string[] split = state.Get<LineContext>("context")!.LineContent
                    .Split(separator)
                    .Where(s => !string.IsNullOrWhiteSpace(s))
                    .ToArray();
            if (split.Length != amountOfValues)
            {
                state.Get<ValidationResult>("validation")!.AddError("");
                split = ["0", "0"];
            }
            return split;
        }
        private static byte fromByte(State state, string valueName, string value, int minLimit = 0, int maxLimit = 255)
        {
            if (!byte.TryParse(value, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out byte result))
                state.Get<ValidationResult>("validation")!.AddError("");
            if (result < minLimit || result > maxLimit)
                state.Get<ValidationResult>("validation")!.AddError("");
            return result;
        }

        [GeneratedRegex(@"[a-wA-W\.]")]
        private static partial Regex exclusivePathCharacters();
    }
}