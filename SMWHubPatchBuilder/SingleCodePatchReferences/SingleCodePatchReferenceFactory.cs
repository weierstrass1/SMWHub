using SMWHubASMCodeLibrary;

namespace SMWHubPatchBuilder.SingleCodePatchReferences
{
    public static class SingleCodePatchReferenceFactory
    {
        public static ISingleCodePatchReference? CreateInstance(ScopeType type)
        {
            switch(type)
            {
                case ScopeType.NormalSprite:
                    return new NormalSpritePatchReference();
                case ScopeType.Sprite:
                case ScopeType.ClusterSprite:
                case ScopeType.ExtendedSprite:
                case ScopeType.MinorExtendedSprite:
                    return new SpritePatchReference();
                case ScopeType.UberASM:
                case ScopeType.GamemodeASM:
                case ScopeType.OverworldASM:
                    return new UberasmPatchReference();
                case ScopeType.LevelASM:
                    return new LevelAsmPatchReference();
                default:
                    return new StandardPatchReference();
            }
        }
    }
}
