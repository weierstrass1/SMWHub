using FormatReadLibrary.Infos;
using FormatReadLibrary.LineContexts;
using FormatReadLibrary.Readers.ParsingContexts;
using FormatReadLibrary.Readers.StateVariables;
using SMWHubEnumerators;
using SMWHubValidations;
using Validations;

namespace FormatReadLibrary.Readers;

public sealed partial class DynamicInfoReader
{
    private sealed class DynamicInfoResourceListParsingContext : ParsingContext
    {
        public required DynamicInfo DynamicInfo { get; init; }
        private FileEnumeratorLineContext _fileEnumeratorLineContext;
        private readonly List<string> _list = [];
        private readonly DynamicInfoSection _section;
        private string _filepath => State.Get<FilePath>("Filepath")!.Path;
        public DynamicInfoResourceListParsingContext(FileEnumeratorLineContext context, string baseDirectory, DynamicInfoSection section) : base(context)
        {
            _fileEnumeratorLineContext = context;
            _section = section;
            State.AddVariable("Filepath", new FilepathStateVariable(baseDirectory, false));
        }
        public override ValidationResult ProcessEntry()
        {
            Context = LineContext;
            ValidationResult result = getSelfValidatedVariables(LineContext.LineContent);
            if (!result)
                return result;

            _list.Add(_filepath);
            if (!_fileEnumeratorLineContext.IsLastLine)
                return result;

            setupDynamicInfoList();
            return result;
        }
        private void setupDynamicInfoList()
        {
            string[] arr = [.. _list];

            switch (_section)
            {
                case DynamicInfoSection.PosesGraphics:
                    DynamicInfo.PoseGraphics = arr;
                    break;
                case DynamicInfoSection.Palettes:
                    DynamicInfo.Palettes = arr;
                    break;
                case DynamicInfoSection.Resources:
                    DynamicInfo.Resources = arr;
                    break;
            }
        }
    }
}
