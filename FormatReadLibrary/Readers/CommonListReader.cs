using FormatReadLibrary.Entries;
using FormatReadLibrary.Logging.LoggingRegisters;
using LogRegister;
using System.Text.RegularExpressions;

namespace FormatReadLibrary.Readers;

public sealed class CommonListReader
{
    private readonly Dictionary<string, string> _baseDirectories;
    private readonly Dictionary<string, Dictionary<int, CommonListEntry>> _entriesList;
    public CommonListReader((string, string)[] titles)
    {
        _baseDirectories = [];
        _entriesList = [];
        string lowertitle;
        foreach ((string title, string baseDir) in titles)
        {
            lowertitle = $"{title.ToLower().Trim()}:";
            _baseDirectories.TryAdd(lowertitle, baseDir);
            _entriesList.TryAdd(lowertitle, []);
        }
    }
    public bool Read(string path, LogRegisterSystem log, int maxID = 255, bool allowVariables = false)
    {
        string content = FileUtils.CleanFileContent(path);

        string[] lines = content.Split('\n');

        ParsingContext ctx = new(path, _entriesList.Keys, log, lines)
        {
            BaseDirectories = _baseDirectories,
            EntriesList = _entriesList,
            MaxID = maxID,
            AllowVariables = allowVariables
        };

        for (int i = 0; i < lines.Length; i++)
        {
            if (string.IsNullOrWhiteSpace(lines[i]))
                continue;
            if (!ctx.ProcessEntry(i))
                return false;
        }
        return true;
    }
    public IEnumerable<CommonListEntry> GetEntries()
    {
        List<CommonListEntry> entries = [];
        foreach (var entry in _entriesList)
        {
            entries.AddRange(entry.Value.OrderBy(kvp => kvp.Key).Select(kvp => kvp.Value));
        }
        return entries;
    }

    private sealed class ParsingContext
    {
        private static readonly Regex _entryRegex = FileRegexContainer.ListEntryRegex();

        public required int MaxID { get; init; }
        public required bool AllowVariables { get; init; }
        public required Dictionary<string, Dictionary<int, CommonListEntry>> EntriesList { get; init; }
        public required Dictionary<string, string> BaseDirectories { get; init; }

        private readonly LogRegisterSystem _log;
        private readonly string _path;
        private readonly string[] _fileContentLines;
        private readonly Dictionary<string, bool> _checkedTitle;
        
        private int _id;
        private string _baseDirectory = "";
        private string? _filepath;
        private string? _title;
        private int[]? _values;
        private Match? _m;
        private Dictionary<int, CommonListEntry>? _currentDic;

        public ParsingContext(string path, IEnumerable<string> titles, LogRegisterSystem log, string[] fileContentLines)
        {
            _path = path;
            _fileContentLines = fileContentLines;
            _checkedTitle = titles.ToDictionary(k => k, k => false);
            _log = log;
        }
        public bool ProcessEntry(int i)
        {
            string lowerLine = _fileContentLines[i].ToLower().Trim();
            if (isATitle(lowerLine))
            {
                if (!validateTitleIsNotRepeated(i, lowerLine))
                    return false;
                _baseDirectory = BaseDirectories[lowerLine];
                _checkedTitle[lowerLine] = true;
                _currentDic = EntriesList[lowerLine];
                _title = lowerLine;
                return true;
            }
            
            return validateAndAddEntry(i);
        }
        private bool validateAndAddEntry(int i)
        {
            if (!validateListContext(i) ||
                !CommonValidations.ValidateEntryFormat(i, _path, _log, _fileContentLines, _entryRegex, out _m) ||
                !validateEntryID(i) ||
                !validateFileExists() ||
                !CommonValidations.ValidateEntryVariables(i, _path, _log, _fileContentLines, out _values, _m, AllowVariables) ||
                !CommonValidations.ValidateDuplicateID(i, _path, _log, _fileContentLines, _id, _currentDic!))
                return false;
            _currentDic!.Add(_id, new()
            {
                EntryType = _title!.Replace(":", ""),
                ID = _id,
                Path = _filepath!,
                Values = _values!
            });
            return true;
        }
        private bool isATitle(string lowerLine)
        {
            return EntriesList.ContainsKey(lowerLine);
        }
        private bool validateTitleIsNotRepeated(int i, string lowerline)
        {
            if (_checkedTitle[lowerline])
            {
                _log.Add(new SyntaxError(_path, i, _fileContentLines[i], "Repeated List Title"));
                return false;
            }
            return true;
        }
        private bool validateListContext(int i)
        {
            if (_currentDic == null)
            {
                _log.Add(new SyntaxError(_path, i, _fileContentLines[i], "List doesn't contain a title"));
                return false;
            }

            return true;
        }
        private bool validateEntryID(int i)
        {
            _id = Convert.ToInt32(_m!.Groups["id"].Value, 16);
            if (_id > MaxID)
            {
                _log.Add(new SyntaxError(_path, i, _fileContentLines[i], $"ID is over the maximum value ({MaxID:X2})"));
                return false;
            }

            return true;
        }
        private bool validateFileExists()
        {
            _filepath = Path.Combine(_baseDirectory, _m!.Groups["file"].Value);
            return CommonValidations.ValidateFileExists(_path, _log, _filepath);
        }
    }
}