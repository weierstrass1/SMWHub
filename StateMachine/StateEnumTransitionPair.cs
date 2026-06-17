using StateMachine.Interfaces;

namespace StateMachine;

public class StateEnumTransitionPair<T>(T idToTransition, ITransition transition) where T : struct, Enum
{
    public readonly T IDToTransition = idToTransition;
    public readonly ITransition Transition = transition;
    public void Deconstruct(out T idToTransition, out ITransition transition)
    {
        idToTransition = IDToTransition;
        transition = Transition;
    }
    public static implicit operator (T, ITransition)(StateEnumTransitionPair<T> pair)
    {
        return (pair.IDToTransition, pair.Transition);
    }
}
