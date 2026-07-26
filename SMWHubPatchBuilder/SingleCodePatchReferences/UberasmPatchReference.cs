namespace SMWHubPatchBuilder.SingleCodePatchReferences;

public class UberasmPatchReference : ISingleCodePatchReference
{
    public virtual string[] DetectedLabels { get => ["main", "init", "end", "nmi"]; }
    public void ProcessSinglePatchOutput(string output)
    {
        throw new NotImplementedException();
    }
}
