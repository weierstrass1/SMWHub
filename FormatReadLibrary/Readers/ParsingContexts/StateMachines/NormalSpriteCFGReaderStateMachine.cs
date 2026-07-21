using FormatReadLibrary.LineContexts;
using System.Text.RegularExpressions;
using Validations;
using ZWXStateMachine;
using ZWXStateMachine.Attributes;
using ZWXStateMachine.Interfaces;
using ZWXStateMachine.StateBehaviours;
using ZWXStateMachine.StateLogics;
using ZWXStateMachine.Transitions;

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
    [RequiresStateVariable("Context", typeof(FileEnumeratorLineContext))]
    [RequiresStateVariable("Validation", typeof(ValidationResult))]
    [RequiresStateVariable("Type", typeof(int))]
    [RequiresStateVariable("ActLike", typeof(byte))]
    [RequiresStateVariable("$1656", typeof(byte))]
    [RequiresStateVariable("$1662", typeof(byte))]
    [RequiresStateVariable("$166E", typeof(byte))]
    [RequiresStateVariable("$167A", typeof(byte))]
    [RequiresStateVariable("$1686", typeof(byte))]
    [RequiresStateVariable("$190F", typeof(byte))]
    [RequiresStateVariable("Prop1", typeof(byte))]
    [RequiresStateVariable("Prop2", typeof(byte))]
    [RequiresStateVariable("FilePath", typeof(string))]
    [RequiresStateVariable("CleanEBAmount", typeof(int))]
    [RequiresStateVariable("SetEBAmount", typeof(int))]
    private partial class NormalSpriteCFGReaderStateMachine : StateMachine<NSCFGStateEnum>
    {
        public NormalSpriteCFGReaderStateMachine(StateData stateData) : base(stateData, NSCFGStateEnum.Type, configTransitions(), configStates(), new EmptyStateBehaviour())
        {
            stateData.AddVariable("PropsDone", false);
        }
        private static Dictionary<NSCFGStateEnum, StateIDBehaviourPair<NSCFGStateEnum>> configStates()
        {
            Dictionary<NSCFGStateEnum, StateIDBehaviourPair<NSCFGStateEnum>> states = [];
            states.Add(NSCFGStateEnum.Type, (NSCFGStateEnum.Type, new NormalSpriteCFGValuesLineStateLogic<int>(
                "Sprite Type Line", ' ', 0, 2, ("Type", "Sprite Type"))));
            states.Add(NSCFGStateEnum.ActLike, (NSCFGStateEnum.ActLike, new NormalSpriteCFGValuesLineStateLogic<byte>(
                "Act Like Line", ' ', 0, 255, ("ActLike", "Act Like"))));
            states.Add(NSCFGStateEnum.Tweaks, (NSCFGStateEnum.Tweaks, new NormalSpriteCFGValuesLineStateLogic<byte>(
                "Tweakers Line", ' ', 0, 255,
                ("$1656", "Tweak 1656"),
                ("$1662", "Tweak 1662"),
                ("$166E", "Tweak 166E"),
                ("$167A", "Tweak 167A"),
                ("$1686", "Tweak 1686"),
                ("$190F", "Tweak 190F"))));
            states.Add(NSCFGStateEnum.Props, (NSCFGStateEnum.Props, new NormalSpriteCFGValuesLineStateLogic<byte>(
                "Properties Line", ' ', 0, 255,
                ("Prop1", "Extra Property 1"),
                ("Prop2", "Extra Property 2"))));
            states.Add(NSCFGStateEnum.File,
                (NSCFGStateEnum.File, new DelegateStateBehaviourEnter(state =>
            {
                if (state.Get<int>("Type") == 0)
                    state.Get<ValidationResult>("Validation")!.AddError("");
                state.Set("FilePath", state.Get<LineContext>("Context")!.LineContent);
            })));
            states.Add(NSCFGStateEnum.ExBytes, (NSCFGStateEnum.ExBytes, new NormalSpriteCFGValuesLineStateLogic<int>(
                "Extra Byte Line", ' ', 0, 12,
                ("CleanEBAmount", "Extra Byte Amount when Extra Bit is Clear"),
                ("SetEBAmount", "Extra Byte Amount when Extra Bit is Set"))));
            return states;
        }
        private static Dictionary<NSCFGStateEnum, List<StateIDTransitionPair<NSCFGStateEnum>>> configTransitions()
        {
            ITransition alwaysTransition = new DelegateTransition(state => true);
            ITransition filenameTransition = new DelegateTransition(state =>
            {
                Regex r = exclusivePathCharacters();
                return r.IsMatch(state.Get<LineContext>("Context")!.LineContent);
            });
            ITransition exByteTransition = new DelegateTransition(state =>
            {
                return state.Get<LineContext>("Context")!.LineContent.Contains(':');
            });
            ITransition propsTransition = new DelegateTransition(state =>
            {
                return state.Get<bool>("PropsDone");
            });
            Dictionary<NSCFGStateEnum, List<StateIDTransitionPair<NSCFGStateEnum>>> transitions = [];
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