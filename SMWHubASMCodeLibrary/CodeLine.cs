using SMWHubASMCodeLibrary.Exceptions;

namespace SMWHubASMCodeLibrary
{
    public class CodeLine(string content, Code root, string filePath, int lineNumber, CodeLine? parent = null)
    {
        public readonly string Content = content;
        public Code Root = root;
        public CodeLine? Parent = parent;
        public string FilePath = filePath;
        public int LineNumber = lineNumber;
        public void GenerateCircularIncludeException()
        {
            CodeLine? current = Parent;
            HashSet<string> included = [];
            while (current != null)
            {
                if (included.Contains(current.FilePath))
                    throw new CircularIncludeException(Parent!);
                included.Add(FilePath);
                current = current.Parent;
            }
        }
        public override string ToString()
        {
            return $"{FilePath} line {LineNumber}: {Content}";
        }
    }
}
