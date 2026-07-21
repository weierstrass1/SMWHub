namespace SMWHubPatchBuilder.SingleCodePatchReferences
{
    public class StandardPatchReference : ISingleCodePatchReference
    {
        public string[] DetectedLabels => [];
        public void ProcessSinglePatchOutput(string output)
        {
            throw new NotImplementedException();
        }
    }
}
