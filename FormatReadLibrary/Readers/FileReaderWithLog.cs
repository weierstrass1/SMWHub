using LogRegister;
using System.Collections;

namespace FormatReadLibrary.Readers
{
    public class FileReaderWithLog(string path, LogRegisterSystem log) : IEnumerable<string>
    {
        public string this[int index]
        {
            get => _fileContentLines[index];
        }
        public int Length => _fileContentLines.Length;
        public readonly LogRegisterSystem Log = log;
        private readonly string _path = path;
        private readonly string[] _fileContentLines = FileUtils.CleanFileContent(path).Split('\n');
        public void AddLog(int i ,Func<int, string, string, ILoggingRegister> registerFunc)
        {
            Log.Add(registerFunc(i, _path, _fileContentLines[i]));
        }
        public IEnumerator<string> GetEnumerator()
        {
            return new FileEnumeratorWithLog(this);
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
