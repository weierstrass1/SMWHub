using FormatLibrary;
using SMWHubASMCodeLibrary.IncludedFiles;
using System.Text.RegularExpressions;

namespace SMWHubASMCodeLibrary;

public partial class Code(string path, CodeType type, CodeScope scope)
{
    public readonly string FilePath = path;
    public readonly string ScopePath = Path.Combine(scope.ScopeDirectoryPath, path);
    public readonly string SourcePath = Path.Combine(scope.SourceDirectoryPath, path);
    public readonly CodeType Type = type;
    public readonly CodeScope Scope = scope;
    public IReadOnlyList<IIncludedFile> RelatedFiles => _relatedFiles.AsReadOnly();
    private readonly List<IIncludedFile> _relatedFiles = [];
    private bool _relatedFilesObtained = false;
    private bool _relatedFilesObtainedFromDynamicInfo = false;
    public IEnumerable<CodeLine> ReadLines(CodeLine? parent = null)
    {
        string fullpath = Path.Combine(Scope.SourceDirectoryPath, FilePath);
        using StreamReader reader = new(fullpath);

        if (!_relatedFilesObtained)
            _relatedFiles.Clear();

        CodeLine codeline;
        string? line;
        int i = 1;
        Match m;
        string filepath;
        IIncludedFile includedFile;

        while ((line = reader.ReadLine()) != null)
        {
            line = FormatCleaner.CleanLine(line);
            codeline = new(line, this, fullpath, i, parent);
            m = incRegex().Match(line);
            if (!m.Success || !_relatedFilesObtained)
            {
                yield return codeline;
                i++;
                continue;
            }
            filepath = m.Groups["filepath1"].Success ?
                m.Groups["filepath1"].Value :
                m.Groups["filepath2"].Value;
            includedFile = IncludedFileFactory.CreateInstance(m.Groups["type"].Value, filepath, i, this);
            _relatedFiles.Add(includedFile);
            yield return codeline;
            i++;
        }
        _relatedFilesObtained = true;
    }
    public IEnumerable<Code> GetRelatedCodes()
    {
        buildChildren();

        Code code;
        foreach (var includedCode in _relatedFiles.OfType<IncludedCode>())
        {
            includedCode.ConvertIntoFile(out code!);
            yield return code;
            foreach(Code c in code.GetRelatedCodes())
            {
                yield return c;
            }
        }
    }
    public override string ToString()
    {
        return $"{Type}-{Scope.Type}: {FilePath}";
    }
    private void buildChildren()
    {
        if (_relatedFilesObtained)
        {
            buildDynamicInfosIncludes();
            return;
        }
        foreach (var _ in ReadLines())
        {
        }
        buildDynamicInfosIncludes();
    }
    private void buildDynamicInfosIncludes()
    {
        if (_relatedFilesObtainedFromDynamicInfo)
            return;
        DynamicInfo di;

        foreach (var includedCode in _relatedFiles.OfType<IncludedDynamicInfo>())
        {
            includedCode.ConvertIntoFile(out di!);
            _relatedFiles.AddRange(di.Palettes
                .Where(p => p.FilePath != null)
                .Select(p => new IncludedBinary(p.FilePath!, includedCode.Line, this)));
            _relatedFiles.AddRange(di.GeneralResources
                .Where(p => p.FilePath != null)
                .Select(p => new IncludedBinary(p.FilePath!, includedCode.Line, this)));
            if (di.PoseGraphics != null)
                _relatedFiles.Add(new IncludedBinary(di.PoseGraphics.FilePath!, includedCode.Line, this));
        }
        _relatedFilesObtainedFromDynamicInfo = true;
    }

    [GeneratedRegex(@"^inc(?<type>(dyni|dri|hbi|pale|hdma|sm|anim)) (""(?<filepath1>[a-zA-Z0-9-_\/\\\.]+)""|(?<filepath2>[a-zA-Z0-9-_\/\\\\.]+))$")]
    private static partial Regex incRegex();
}
