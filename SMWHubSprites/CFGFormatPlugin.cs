using OneOf;
using SMWHubASMCodeLibrary;
using SMWHubEnumerators;
using SMWHubPluginAPI;
using SMWHubSprites.Formats;
using SMWHubSprites.ScopeTypes;
using Validations;

namespace SMWHubSprites;

public class CFGFormatPlugin : IFormatPlugin<SpriteConfigEntry>
{
    private readonly FormatDefinition[] _formatDefinitions = 
        [
            new("CFG", null, ".cfg"), 
            new("JsonCFG", null, ".json")
        ];
    public IEnumerable<FormatDefinition> FormatDefinitions
    {
        get
        {
            foreach (var f in _formatDefinitions)
                yield return f;
        }
    }
    public bool CanBeEmbeddedFrom(Type t)
    {
        return t == typeof(NormalSpriteType);
    }
    public bool CanBeIncludedFrom(Type t)
    {
        return false;
    }
    public IEnumerable<OneOf<ValidationResult, SpriteConfigEntry>> Read(PluginContext context, CodeScope scope, IFormattedEnumerable readerEnum)
    {
        throw new NotImplementedException();
    }
    public ValidationResult Process(PluginContext context, CodeScope scope, IEnumerable<SpriteConfigEntry> obj)
    {
        throw new NotImplementedException();
    }
}
