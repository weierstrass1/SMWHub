using FormatLibrary.Entries;
using FormatReadLibrary.LineContexts;
using StateMachine;
using StateMachine.Attributes;
using StateMachine.Interfaces;
using StateMachine.StateLogics;
using System.Text.RegularExpressions;
using Validations;

namespace FormatReadLibrary.Readers;

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
    [RequiresStateVariable("Context", typeof(LineContext))]
    [RequiresStateVariable("Validation", typeof(ValidationResult))]
    [RequiresStateVariable("Type", typeof(int))]
    [RequiresStateVariable("ActLike", typeof(byte))]
    [RequiresStateVariable("$1656", typeof(Tweak1656))]
    [RequiresStateVariable("$1662", typeof(Tweak1656))]
    [RequiresStateVariable("$166E", typeof(Tweak1656))]
    [RequiresStateVariable("$167A", typeof(Tweak1656))]
    [RequiresStateVariable("$1686", typeof(Tweak1656))]
    [RequiresStateVariable("$190F", typeof(Tweak1656))]
    [RequiresStateVariable("Prop1", typeof(byte))]
    [RequiresStateVariable("Prop2", typeof(byte))]
    [RequiresStateVariable("FilePath", typeof(string))]
    [RequiresStateVariable("CleanEBAmount", typeof(int))]
    [RequiresStateVariable("SetEBAmount", typeof(int))]
    private partial class NormalSpriteCFGReaderStateMachine : StateMachine<NSCFGStateEnum>
    {
        public NormalSpriteCFGReaderStateMachine(State state) : base(state, NSCFGStateEnum.Type, configTransitions(), configStates(), new StateLogic<NSCFGStateEnum>(NSCFGStateEnum.Done, false))
        {
            state.AddVariable("PropsDone", false);
        }
        private static Dictionary<NSCFGStateEnum, IStateLogic<NSCFGStateEnum>> configStates()
        {
            Dictionary<NSCFGStateEnum, IStateLogic<NSCFGStateEnum>> states = [];
            states.Add(NSCFGStateEnum.Type, new NormalSpriteCFGValuesLineStateLogic<int>(NSCFGStateEnum.Type,
                "Sprite Type Line", ' ', 0, 2, ("Type", "Sprite Type")));
            states.Add(NSCFGStateEnum.ActLike, new NormalSpriteCFGValuesLineStateLogic<byte>(NSCFGStateEnum.Type,
                "Act Like Line", ' ', 0, 255, ("ActLike", "Act Like")));
            states.Add(NSCFGStateEnum.Tweaks, new NormalSpriteCFGValuesLineStateLogic<TweakNumber>(NSCFGStateEnum.Type,
                "Tweakers Line", ' ', TweakNumber.Zero, TweakNumber.MaxTweakNumber,
                (typeof(Tweak1656), "$1656", "Tweak 1656"),
                (typeof(Tweak1662), "$1662", "Tweak 1662"),
                (typeof(Tweak166E), "$166E", "Tweak 166E"),
                (typeof(Tweak167A), "$167A", "Tweak 167A"),
                (typeof(Tweak1686), "$1686", "Tweak 1686"),
                (typeof(Tweak190F), "$190F", "Tweak 190F")));
            states.Add(NSCFGStateEnum.Props, new NormalSpriteCFGValuesLineStateLogic<byte>(NSCFGStateEnum.Type,
                "Properties Line", ' ', 0, 255,
                ("Prop1", "Extra Property 1"),
                ("Prop2", "Extra Property 2")));
            states.Add(NSCFGStateEnum.File,
                new DelegateStateLogicStart<NSCFGStateEnum>(NSCFGStateEnum.Type, state =>
            {
                if(state.Get<int>("Type") == 0)
                    state.Get<ValidationResult>("validation")!.AddError("");
                state.Set("FilePath", state.Get<LineContext>("context")!.LineContent);
            }));
            states.Add(NSCFGStateEnum.ExBytes, new NormalSpriteCFGValuesLineStateLogic<int>(NSCFGStateEnum.Type,
                "Extra Byte Line", ' ', 0, 12,
                ("CleanEBAmount", "Extra Byte Amount when Extra Bit is Clear"),
                ("SetEBAmount", "Extra Byte Amount when Extra Bit is Set")));
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

        [GeneratedRegex(@"[a-wA-W\.]")]
        private static partial Regex exclusivePathCharacters();
    }
}