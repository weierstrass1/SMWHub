using FormatLibrary.Interfaces;
using FormatLibrary.ResourceManagement.ResourceTypes;
using ResourceManagement;
namespace FormatLibrary.ResourceManagement;

public class GeneralResource : DataResource, IResourceFactory<GeneralResource>
{
    public GeneralResource(string name, byte[] data) : base(name, new GeneralResourceType(), data)
    {
    }
    public GeneralResource(string filepath) : base(filepath, new GeneralResourceType())
    {
    }
    public static GeneralResource Create(string name, byte[] data)
    {
        return new(name, data);
    }
}
