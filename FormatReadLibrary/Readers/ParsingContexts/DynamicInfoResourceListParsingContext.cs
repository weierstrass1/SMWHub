using FormatReadLibrary.Infos;
using FormatReadLibrary.Readers.ParsingContexts;
using FormatReadLibrary.Readers.StateVariables;

namespace FormatReadLibrary.Readers;

public sealed partial class DynamicInfoReader
{
    private sealed class DynamicInfoResourceListParsingContext : ParsingContext
    {
        public required DynamicInfo DynamicInfo { get; init; }
        private readonly List<string> _list = [];
        private readonly DynamicInfoSection _section;
        private string _filepath => State.Get<FilePath>("Filepath")!.Path;
        public DynamicInfoResourceListParsingContext(FileEnumeratorWithLog fileEnumerator, string baseDirectory, DynamicInfoSection section) : base(fileEnumerator)
        {
            _section = section;
            State.AddVariable("Filepath", new FilepathStateVariable(baseDirectory, false));
        }
        public override bool ProcessEntry()
        {
            if (!getSelfValidatedVariables(FileEnumerator.Current))
                return false;

            _list.Add(_filepath);
            if (!FileEnumerator.IsLastLine())
                return true;

            setupDynamicInfoList();
            return true;
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
