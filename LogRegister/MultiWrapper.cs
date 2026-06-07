namespace LogRegister;

public sealed class MultiWrapper
{
    public event LogRenderAction Actions;
    public void RenderAction(string text, ILogCategory category, SpanType type, bool mustWrite = false)
    {
        Actions?.Invoke(text, category, type, mustWrite: mustWrite);
    }
}
