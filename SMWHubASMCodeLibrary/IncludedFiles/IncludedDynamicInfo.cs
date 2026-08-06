using FormatLibrary;
using FormatReadLibrary.Readers;
using Validations;

namespace SMWHubASMCodeLibrary.IncludedFiles;

public class IncludedDynamicInfo(string filename, int line, Code parent) : IIncludedFile<DynamicInfo>
{
    public int Line { get; private init; } = line;
    public Code Parent { get; private init; } = parent;
    public string IncName => "dyni";
    public string Filename { get; private init; } = filename;
    public ValidationResult ConvertIntoFile(out DynamicInfo? di)
    {
        return DynamicInfoReader.Read(Filename, "", out di);
    }
}
