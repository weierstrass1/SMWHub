namespace SMWHubASMCodeLibrary;

public class ASMCode(string filepath, CodeType type)
{
    public string FilePath = filepath;
    public CodeType Type = type;
    public List<EmbeddedFile> EmbeddedFiles = [];
}
