namespace SMWHubPatchBuilder.SingleCodePatchReferences;

public class LevelAsmPatchReference : UberasmPatchReference
{
    public override string[] DetectedLabels => [ .. base.DetectedLabels , "load"];
}
