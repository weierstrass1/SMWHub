using FormatReadLibrary.Infos;
using FormatReadLibrary.Readers.ParsingContexts;
using FormatReadLibrary.Readers.StateVariables;
using StateMachine;
using System.Text.RegularExpressions;

namespace FormatReadLibrary.Readers;

public sealed partial class DynamicInfoReader
{
    private sealed class DynamicInfoLegacyFormatParsingContext : ParsingContext
    {
        public required DynamicInfo DynamicInfo { get; init; }
        private static Regex _entryRegex = RegexContainer.DynInfoLegacyRegex();
        private static Regex _entryTableRegex = RegexContainer.NumberTableRegex();
        private static readonly MatchStateVariable _matchTitle = new("Match", _entryRegex);
        private readonly Dictionary<string, (int, int)> _poseChunkSizes = [];
        private readonly ValidateIfHasNext _ifHasNext;
        private readonly ValidateDuplicateID<string, (int, int)> _validateDuplicateID;
        public DynamicInfoLegacyFormatParsingContext(FileEnumeratorWithLog fileEnumerator) : base(fileEnumerator)
        {
            State.AddVariable("MatchTable", new MatchStateVariable("MatchTable", _entryTableRegex));
            State.AddVariable("ID", new StateVariable<string>());
            State.AddVariable("Values", new StateVariable<int[]>());
            _ifHasNext = new ValidateIfHasNext(fileEnumerator);
            _validateDuplicateID = new ValidateDuplicateID<string, (int, int)>(this, _poseChunkSizes);
            addValidator(new ValidateTableValueSize(() => fileEnumerator.Current, TableValueSize.db));
            addValidator(new ValidateValuesSize(this, 2, 2));
        }
        public override bool ProcessEntry()
        {
            string id;
            if (!validatePoseChunksTitle(out id))
                return false;

            if (!_ifHasNext.Validate(this))
                return false;
            _ifHasNext.MoveToTheNextNotEmptyLine();

            if (!getSelfValidatedVariables(FileEnumerator.Current))
                return false;

            int[]? values = setupValues();
            if (!validate())
                return false;

            _poseChunkSizes.Add(id, (values![0], values[1]));
            if (!FileEnumerator.IsLastLine())
                return true;

            setupDynamicInfoPosesChunksSizes();
            return true;
        }
        private bool validatePoseChunksTitle(out string id)
        {
            ValidationResult result = _matchTitle.GetFrom(FileEnumerator.Current);
            if (!result.IsValid)
            {
                ValidatorLogAdapter.LogValidatorResult(FileEnumerator, result);
                id = "";
                return false;
            }

            Match match = _matchTitle.Value!;

            id = match.Groups["id"].Value;
            State.Set("ID", id);

            result.Merge(_validateDuplicateID.Validate(this));

            if (!result.IsValid)
                ValidatorLogAdapter.LogValidatorResult(FileEnumerator, result);

            return result;
        }
        private int[] setupValues()
        {
            var valuesVar = State.GetVariable<StateVariable<int[]>>("Values");
            valuesVar.Value = HexUtils.GetValues(FileEnumerator.Current);
            return valuesVar.Value;
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
