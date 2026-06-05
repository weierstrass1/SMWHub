using FormatReadLibrary.Infos;
using FormatReadLibrary.Readers.Enumerators;
using FormatReadLibrary.Readers.ParsingContexts;
using FormatReadLibrary.Readers.Validators;

namespace FormatReadLibrary.Readers;

public sealed partial class DynamicInfoReader
{
    private sealed class DynamicInfoResourceListParsingContext : ParsingContext
    {
        public required DynamicInfo DynamicInfo { get; init; }
        private readonly List<string> _list = [];
        private readonly DynamicInfoSection _section;
        public DynamicInfoResourceListParsingContext(FileEnumeratorWithLog fileEnumerator, DynamicInfoSection section) : base(fileEnumerator)
        {
            _section = section;
            addValidator(new ValidatePathIntegrity(this, FileEnumerator));
        }
        public override bool ProcessEntry()
        {
            if (!validate())
                return false;

            _list.Add(FileEnumerator.Current);
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
