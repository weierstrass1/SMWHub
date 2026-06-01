using FormatReadLibrary.Infos;

namespace FormatReadLibrary.Readers;

public interface IHaveDynamicInfo
{
    public DynamicInfo DynamicInfo { get; init; }
}
