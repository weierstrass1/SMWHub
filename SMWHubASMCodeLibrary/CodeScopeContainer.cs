using SMWHubASMCodeLibrary.DTO;

namespace SMWHubASMCodeLibrary
{
    public class CodeScopeContainer
    {
        public CodeScope this[ScopeType index]
        {
            get => _scopes[index];
        }
        private readonly IReadOnlyDictionary<ScopeType, CodeScope> _scopes;
        public CodeScopeContainer(string path)
        {
            FolderContainer folders = FolderContainer.GetFromJson(path);
            SpriteFolderContainer sprDir = folders.SpritesFolders;
            UberasmFolderContainer uasmDir = folders.UberasmFolder;
            CodeScope main = new(folders.Main, ScopeType.Global);
            CodeScope sprite = new(sprDir.Main, ScopeType.Sprite, main);
            CodeScope uberasm = new(uasmDir.Main, ScopeType.UberASM, main);
            _scopes = new Dictionary<ScopeType, CodeScope>()
            {
                { ScopeType.Global, main },
                { ScopeType.Sprite, sprite },
                { ScopeType.NormalSprite,new(Path.Combine(sprDir.Main, sprDir.NormalSpritesFolder), ScopeType.NormalSprite, sprite) },
                { ScopeType.ClusterSprite,new(Path.Combine(sprDir.Main, sprDir.ClusterSpritesFolder), ScopeType.ClusterSprite, sprite) },
                { ScopeType.ExtendedSprite,new(Path.Combine(sprDir.Main, sprDir.ExtendedSpritesFolder), ScopeType.ExtendedSprite, sprite) },
                { ScopeType.UberASM,uberasm },
                { ScopeType.LevelASM, new(Path.Combine(uasmDir.Main, uasmDir.LevelFolder), ScopeType.LevelASM, uberasm) },
                { ScopeType.OverworldASM,new(Path.Combine(uasmDir.Main, uasmDir.OverworldFolder), ScopeType.OverworldASM, uberasm) },
                { ScopeType.GamemodeASM,new(Path.Combine(uasmDir.Main, uasmDir.GameModeFolder), ScopeType.GamemodeASM, uberasm) },
                { ScopeType.OverworldSprite, new(folders.OverworldSpritesFolder, ScopeType.OverworldSprite, main) },
                { ScopeType.Block,new(folders.BlocksFolder, ScopeType.Block, main) },
                { ScopeType.Patch,new(folders.PatchesFolder, ScopeType.Patch, main) }
            }.AsReadOnly();
        }
    }
}
