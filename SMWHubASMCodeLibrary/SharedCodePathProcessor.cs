namespace SMWHubASMCodeLibrary;

public enum CodeType
{
    Macros,
    Defines,
    Routines,
    Code
};
public class SharedCodePathProcessor
{
    public const string SHARED_CODE_DIRECTORY = "_SharedCode";
    private readonly CodeContext _context;
    public SharedCodePathProcessor(CodeContext ctx)
    {
        _context = ctx;
    }
    public List<Code> FindSharedCodes()
    {
        return [.. _context.Scopes
            .SelectMany(getSharedCodeTypesFiles)];
    }
    private IEnumerable<Code> getSharedCodeTypesFiles(CodeScope scope)
    {
        return Enum.GetValues<CodeType>()
            .Where(t => t != CodeType.Code)
            .SelectMany(t => getFiles(scope, t));
    }
    private IEnumerable<Code> getFiles(CodeScope scope, CodeType type)
    {
        string tDirectory = Path.Combine(scope.SourceDirectoryPath, SHARED_CODE_DIRECTORY, type.ToString());
        return ensureDirectoryAndGetFiles(scope, tDirectory)
            .Select(f => new Code(f, type, scope));
    }
    private IEnumerable<string> ensureDirectoryAndGetFiles(CodeScope scope, string tDirectory)
    {
        Directory.CreateDirectory(tDirectory);
        return Directory.EnumerateFiles(tDirectory, "*.asm", SearchOption.AllDirectories)
            .Select(fp => string.IsNullOrWhiteSpace(scope.SourceDirectoryPath) ?
                fp :
                Path.GetRelativePath(scope.SourceDirectoryPath, fp));
    }
}
