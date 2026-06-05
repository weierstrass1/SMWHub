using FormatReadLibrary.Infos;
using FormatReadLibrary.Readers.Enumerators;
using FormatReadLibrary.Readers.ParsingContexts;
using FormatReadLibrary.Readers.StateVariables;
using FormatReadLibrary.Readers.Validators;
using System.Text.RegularExpressions;

namespace FormatReadLibrary.Readers;

public sealed partial class DynamicInfoReader
{
    private sealed class DynamicInfoCurrentFormatParsingContext : ParsingContext
    {
        public required DynamicInfo DynamicInfo { get; init; }
        private static Regex _entryRegex = FileRegexContainer.DynInfoCurrentRegex();
        private readonly Dictionary<int, string> _currentNumberOf16x16TilesPerPose = [];
        public DynamicInfoCurrentFormatParsingContext(FileEnumeratorWithLog fileEnumerator) : base(fileEnumerator)
        {
            State.AddVariable("Match", new MatchStateVariable());
            State.AddVariable("IDs", new ValuesStateVariable());
            addValidator(new ValidateEntryFormat(this, FileEnumerator));
        }

        public override bool ProcessEntry()
        {
            Match match = setupMatch();

            if (!validate())
                return false;

            addValues(match);

            if (!FileEnumerator.IsLastLine())
                return true;

            DynamicInfo.FromNumberOf16x16Tiles(_currentNumberOf16x16TilesPerPose);
            return true;
        }
        private void addValues(Match match)
        {
            var idsVar = State.GetVariable<ValuesStateVariable>("IDs");
            int[] ids = idsVar.GetFrom(match)!;

            string value = $"{match.Groups["tiles"]}{match.Groups["modifier"]}";

            foreach (int id in ids)
            {
                _currentNumberOf16x16TilesPerPose[id] = value;
            }
        }
        private Match setupMatch()
        {
            var matchVar = State.GetVariable<MatchStateVariable>("Match");
            Match match = matchVar.GetFrom(FileEnumerator.Current, _entryRegex)!;
            return match;
        }
    }
}
