namespace SMWHubPatchBuilder.SingleCodePatchReferences;

public interface ISingleCodePatchReference
{
    public string[] DetectedLabels { get; }
    public void ProcessSinglePatchOutput(string output);
}
