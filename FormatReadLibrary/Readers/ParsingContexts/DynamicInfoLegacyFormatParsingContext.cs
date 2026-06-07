using FormatReadLibrary.Infos;
using FormatReadLibrary.LineContexts;
using FormatReadLibrary.Readers.ParsingContexts;
using FormatReadLibrary.Readers.StateVariables;
using SMWHubEnumerators;
using SMWHubValidations;
using StateMachine;
using System.Text.RegularExpressions;
using Validations;

namespace FormatReadLibrary.Readers;

public sealed partial class DynamicInfoReader
{
    private sealed class DynamicInfoLegacyFormatParsingContext : ParsingContext
    {
        public required DynamicInfo DynamicInfo { get; init; }
        private readonly FileEnumeratorLineContext _fileEnumeratorLineContext;
        private static readonly Regex _entryRegex = RegexContainer.DynInfoLegacyRegex();
        private static readonly Regex _entryTableRegex = RegexContainer.NumberTableRegex();
        private static readonly MatchStateVariable _matchTitle = new("Match", _entryRegex);
        private readonly Dictionary<string, (int, int)> _poseChunkSizes = [];
        private readonly ValidateIfHasNext _ifHasNext;
        private readonly ValidateDuplicateID<string, (int, int)> _validateDuplicateID;
        public DynamicInfoLegacyFormatParsingContext(FileEnumeratorLineContext context) : base(context)
        {
            _fileEnumeratorLineContext = context;
            State.AddVariable("MatchTable", new MatchStateVariable("MatchTable", _entryTableRegex));
            State.AddVariable("ID", new StateVariable<string>());
            State.AddVariable("Values", new StateVariable<int[]>());
            _ifHasNext = new ValidateIfHasNext(_fileEnumeratorLineContext);
            _validateDuplicateID = new ValidateDuplicateID<string, (int, int)>(this, _poseChunkSizes);
            addValidator(new ValidateTableValueSize(() => LineContext.LineContent, TableValueSize.db));
            addValidator(new ValidateValuesSize(this, 2, 2));
        }
        public override ValidationResult ProcessEntry()
        {
            Context = LineContext;
            ValidationResult result = validatePoseChunksTitle(Context, out string id);
            if (!result)
                return result;

            result = _ifHasNext.Validate(this);
            if (!result)
                return result;
            _ifHasNext.MoveToTheNextNotEmptyLine();

            result = getSelfValidatedVariables(LineContext.LineContent);
            if (!result)
                return result;

            int[]? values = setupValues();

            result = validate();
            if (!result)
                return result;

            _poseChunkSizes.Add(id, (values![0], values[1]));
            if (!_fileEnumeratorLineContext.IsLastLine)
                return result;

            setupDynamicInfoPosesChunksSizes();
            return result;
        }
        private ValidationResult validatePoseChunksTitle(ValidationContext context, out string id)
        {
            ValidationResult result = _matchTitle.GetFrom(context, LineContext.LineContent);
            if (!result.IsValid)
            {
                id = "";
                return result;
            }

            Match match = _matchTitle.Value!;

            id = match.Groups["id"].Value;
            State.Set("ID", id);

            result.Merge(_validateDuplicateID.Validate(this));

            return result;
        }
        private int[] setupValues()
        {
            var valuesVar = State.GetVariable<StateVariable<int[]>>("Values");
            valuesVar.Value = HexUtils.GetValues(LineContext.LineContent);
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
