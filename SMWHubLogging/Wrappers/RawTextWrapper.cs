using LogRegister;
using LogRegister.Interfaces;
using System.Text;

namespace SMWHubLogging.Wrappers;
public sealed class RawTextWrapper : ILogWrapper
{
    private readonly StringBuilder _builder;
    public RawTextWrapper()
    {
        _builder = new();
    }
    public void RenderAction(string text, ILogCategory category, SpanType type, bool mustWrite = false)
    {
        if (type == SpanType.Prefix)
            return;
        _builder.Append(text);
    }
    public override string ToString()
    {
        return _builder.ToString();
    }
}
