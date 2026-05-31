using FormatReadLibrary.Entries;
using FormatReadLibrary.Readers.Validators;
using LogRegister;
using StateMachine;
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

        CommonListParsingContext ctx = new(path, _entriesList.Keys, log, lines, maxID, allowVariables)
        {
            BaseDirectories = _baseDirectories,
            EntriesList = _entriesList,
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
    private sealed class CommonListParsingContext : ParsingContext
    {
        private static readonly Regex _entryRegex = FileRegexContainer.ListEntryRegex();
        public required Dictionary<string, Dictionary<int, CommonListEntry>> EntriesList { get; init; }
        public required Dictionary<string, string> BaseDirectories { get; init; }
        private readonly ValidateTitleIsNotRepeated _titleIsNotRepeated;
        private string? _title;
        public CommonListParsingContext(string path, IEnumerable<string> titles, LogRegisterSystem log, string[] fileContentLines, int maxID, bool allowVariables) : base()
        {
            State.AddVariable("BaseDirectory", new StateVariable<string>());
            State.AddVariable("LineIndex", new StateVariable<int>());
            State.AddVariable("Dictionary", new StateVariable<Dictionary<int, CommonListEntry>>());
            State.AddVariable("Log", new StateVariable<LogRegisterSystem>()
            {
                Value = log
            });
            State.AddVariable("Path", new StateVariable<string>()
            {
                Value = path
            });
            State.AddVariable("FileContentLines", new StateVariable<string[]>()
            {
                Value = fileContentLines
            });
            State.AddVariable("MaxID", new StateVariable<int>()
            {
                Value = maxID
            });
            State.AddVariable("AllowVariables", new StateVariable<bool>()
            {
                Value = allowVariables
            });
            State.AddVariable("CheckedTitle", new StateVariable<Dictionary<string, bool>>()
            {
                Value = titles.ToDictionary(k => k, k => false)
            });
            State.AddVariable("Match", new LazyStateVariable<Match>(() =>
            {
                var i = State.Get<int>("LineIndex")!;
                var fileContentLines = State.Get<string[]>("FileContentLines")!;
                return _entryRegex.Match(fileContentLines[i]);
            }));
            State.AddVariable("ID", new LazyStateVariable<int?>(() =>
            {
                var match = State.Get<Match>("Match")!;
                return Convert.ToInt32(match.Groups["id"].Value, 16);
            }));
            State.AddVariable("Filepath", new LazyStateVariable<string>(() =>
            {
                var match = State.Get<Match>("Match")!;
                var baseDirectory = State.Get<string>("BaseDirectory")!;
                return Path.Combine(baseDirectory, match!.Groups["file"].Value);
            }));
            State.AddVariable("Values", new LazyStateVariable<int[]>(() =>
            {
                var match = State.Get<Match>("Match")!;
                if (!match.Groups["var"].Success)
                    return [];
                return [..match.Groups["var"].Value
                    .Split(' ')
                    .Select(x => x[0] == '@' ?
                        int.Parse(x[1..]) :
                        Convert.ToInt32(x, 16))];
            }));
            _titleIsNotRepeated = new(this);
            AddValidator(new ValidateListContext<CommonListEntry>(this));
            AddValidator(new ValidateEntryFormat(this));
            AddValidator(new ValidateEntryID(this));
            //AddValidator(new ValidateFileExists(this));
            AddValidator(new ValidateEntryVariables(this));
            AddValidator(new ValidateDuplicateID<CommonListEntry>(this));
        }
        public override bool ProcessEntry(int i)
        {
            State.Set("LineIndex", i);
            var fileContentLines = State.Get<string[]>("FileContentLines");
            string lowerLine = fileContentLines[i].ToLower().Trim();
            if (isATitle(lowerLine))
            {
                if (!_titleIsNotRepeated.Validate(this))
                    return false;
                State.Set("BaseDirectory", BaseDirectories[lowerLine]);
                var checkedTitle = State.Get<Dictionary<string, bool>>("CheckedTitle")!;
                checkedTitle[lowerLine] = true;
                State.Set("Dictionary", EntriesList[lowerLine]);
                _title = lowerLine;
                return true;
            }
            if (!validate())
                return false;
            var dictionary = State.Get<Dictionary<int, CommonListEntry>>("Dictionary");
            var id = State.Get<int>("ID");
            dictionary!.Add(id, new()
            {
                EntryType = _title!.Replace(":", ""),
                ID = id,
                Path = State.Get<string>("Filepath")!,
                Values = State.Get<int[]>("Values")!,
            });
            State.CleanLazyTypes();
            return true;
        }
        private bool isATitle(string lowerLine)
        {
            return EntriesList.ContainsKey(lowerLine);
        }
    }
}