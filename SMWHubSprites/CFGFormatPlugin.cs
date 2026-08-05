using OneOf;
using SMWHubEnumerators;
using SMWHubPluginAPI;
using SMWHubSprites.Formats;
using SMWHubSprites.ScopeTypes;
using Validations;

namespace SMWHubSprites;

public class CFGFormatPlugin : IFormatPlugin<SpriteConfigEntry>
{
    private static readonly string[] _embeddedNames = ["CFG", "JsonCFG"];
    private static readonly string[] _fileExtensions = [".cfg", ".json"];
    public IEnumerable<string> EmbeddedNames => _embeddedNames.AsReadOnly();
    public IEnumerable<string> IncludeDirectiveNames { get; } = [];
    public IEnumerable<string> FileExtensions => _fileExtensions.AsReadOnly();
    public bool CanBeEmbeddedFrom(Type t)
    {
        return t == typeof(NormalSpriteType);
    }
    public bool CanBeIncludedFrom(Type t)
    {
        return false;
    }
    public IEnumerable<OneOf<ValidationResult, SpriteConfigEntry>> Read(PluginContext context, IFormattedEnumerable readerEnum)
    {
        throw new NotImplementedException();
    }
    public ValidationResult Process(PluginContext context, IEnumerable<SpriteConfigEntry> obj)
    {
        throw new NotImplementedException();
    }
}
