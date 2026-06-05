using FormatReadLibrary.Infos;
using FormatReadLibrary.Logging.Enumerators;
using FormatReadLibrary.Readers.ParsingContexts;
using FormatReadLibrary.Readers.StateVariables;
using FormatReadLibrary.Readers.Validators;
using StateMachine;
using System.Text.RegularExpressions;

namespace FormatReadLibrary.Readers;

public sealed partial class DynamicInfoReader
{
    private sealed class DynamicInfoLegacyFormatParsingContext : ParsingContext
    {
        public required DynamicInfo DynamicInfo { get; init; }
        private static Regex _entryRegex = FileRegexContainer.DynInfoLegacyRegex();
        private static Regex _entryTableRegex = FileRegexContainer.NumberTableRegex();
        private readonly Dictionary<string, (int, int)> _poseChunkSizes = [];
        private readonly ValidateEntryFormat _validateEntryFormat;
        private readonly ValidateIfHasNext _ifHasNext;
        private readonly ValidateDuplicateID<string, (int, int)> _validateDuplicateID;
        public DynamicInfoLegacyFormatParsingContext(FileEnumeratorWithLog fileEnumerator) : base(fileEnumerator)
        {
            State.AddVariable("Match", new MatchStateVariable());
            State.AddVariable("MatchTable", new MatchStateVariable());
            State.AddVariable("ID", new StateVariable<string>());
            State.AddVariable("Values", new ValuesStateVariable());
            _validateEntryFormat = new ValidateEntryFormat(this, FileEnumerator);
            _validateDuplicateID = new ValidateDuplicateID<string, (int, int)>(this, FileEnumerator, _poseChunkSizes);
            _ifHasNext = new ValidateIfHasNext(this, FileEnumerator);
            addValidator(new ValidateTableValueSize(this, FileEnumerator, TableValueSize.db));
            addValidator(new ValidateEntryFormat(this, FileEnumerator, "MatchTable"));
            addValidator(new ValidateValuesSize(this, FileEnumerator, 2, 2));

        }
        public override bool ProcessEntry()
        {
            string id;
            if (!validatePoseChunksTitle(out id))
                return false;

            if (!_ifHasNext.Validate(this))
                return false;
            _ifHasNext.MoveToTheNextNotEmptyLine();

            int[]? values = setupValues();
            if (!validate())
                return false;

            _poseChunkSizes.Add(id, (values![0], values[1]));
            if (!FileEnumerator.IsLastLine())
                return true;

            setupDynamicInfoPosesChunksSizes();
            return true;
        }
        private int[]? setupValues()
        {
            var matchTableVar = State.GetVariable<MatchStateVariable>("MatchTable");
            matchTableVar.GetFrom(FileEnumerator.Current, _entryTableRegex);
            var valuesVar = State.GetVariable<ValuesStateVariable>("Values");
            return valuesVar.GetFrom(FileEnumerator.Current);
        }
        private bool validatePoseChunksTitle(out string id)
        {
            var matchVar = State.GetVariable<MatchStateVariable>("Match");
            Match match = matchVar.GetFrom(FileEnumerator.Current, _entryRegex)!;

            id = "";

            if (!_validateEntryFormat.Validate(this))
                return false;

            id = match.Groups["id"].Value;
            State.Set("ID", id);

            return _validateDuplicateID.Validate(this);
        }
        private void setupDynamicInfoPosesChunksSizes()
        {
            List<int> pcs = [];
            foreach (var tuple in _poseChunkSizes.Values)
                pcs.AddRange([tuple.Item1, tuple.Item2]);
            DynamicInfo.PosesChunksSizes = [.. pcs];
            DynamicInfo.GenerateLastRow();
        }
    }
}
