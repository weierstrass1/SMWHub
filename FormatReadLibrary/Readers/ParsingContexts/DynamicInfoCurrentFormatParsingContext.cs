using FormatReadLibrary.Infos;
using FormatReadLibrary.LineContexts;
using FormatReadLibrary.Readers.ParsingContexts;
using FormatReadLibrary.Readers.StateVariables;
using SMWHubEnumerators;
using System.Text.RegularExpressions;
using Validations;

namespace FormatReadLibrary.Readers;

public sealed partial class DynamicInfoReader
{
    private sealed class DynamicInfoCurrentFormatParsingContext : ParsingContext
    {
        public required DynamicInfo DynamicInfo { get; init; }
        private FileEnumeratorLineContext _fileEnumeratorLineContext;
        private readonly static Regex _entryRegex = RegexContainer.DynInfoCurrentRegex();
        private readonly Dictionary<int, string> _currentNumberOf16x16TilesPerPose = [];
        private Match _match => State.Get<Match>("Match")!;
        public DynamicInfoCurrentFormatParsingContext(FileEnumeratorLineContext context) : base(context)
        {
            _fileEnumeratorLineContext = context;
            State.AddVariable("Match", new MatchStateVariable("Match", _entryRegex));
            State.AddVariable("IDs", new IntegerIDListStateVariable<string>(_currentNumberOf16x16TilesPerPose, 1000, true, false));
        }
        public override ValidationResult ProcessEntry()
        {
            Context = LineContext;
            ValidationResult result = getSelfValidatedVariables(LineContext.LineContent);
            if (!result)
                return result;

            addValues();

            if (!_fileEnumeratorLineContext.IsLastLine)
                return result;

            DynamicInfo.FromNumberOf16x16Tiles(_currentNumberOf16x16TilesPerPose);
            return result;
        }
        private void addValues()
        {
            var idsVar = State.GetVariable<IntegerIDListStateVariable<string>>("IDs");
            int[] ids = idsVar.Value!;

            string value = $"{_match.Groups["tiles"]}{_match.Groups["modifier"]}";

            foreach (int id in ids)
            {
                _currentNumberOf16x16TilesPerPose[id] = value;
            }
        }
    }
}
