namespace SMWHubASMCodeLibrary.IncludedFiles
{
    public static class IncludedFileFactory
    {
        private static readonly Dictionary<string, Func<string, int, Code, IIncludedFile>> _includedFiles = new()
        {
            {"dyni", static (filename, line, parent) => new IncludedDynamicInfo(filename, line, parent) },
            {"bin",  static (filename, line, parent) => new IncludedBinary(filename, line, parent)},
            {"src",  static (filename, line, parent) => new IncludedCode(filename,line,parent)}
        };
        public static IIncludedFile CreateInstance(string incName, string filename, int line, Code parent)
        {
            return _includedFiles[incName](filename, line, parent);
        }
    }
}
