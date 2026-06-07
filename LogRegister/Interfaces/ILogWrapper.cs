namespace LogRegister.Interfaces;

public interface ILogWrapper
{
    public void RenderAction(string text, ILogCategory category, SpanType type, bool mustWrite = false);
}
