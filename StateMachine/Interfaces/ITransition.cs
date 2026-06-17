namespace StateMachine.Interfaces;

public interface ITransition
{
    public bool MustTransition(State state);
}
