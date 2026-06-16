using FormatLibrary.Interfaces;
using FormatLibrary.ResourceManagement.ResourceTypes;
using ResourceManagement;
namespace FormatLibrary.ResourceManagement;

public class PaletteResource : DataResource, IResourceFactory<PaletteResource>
{
    public PaletteResource(string name, byte[] data) : base(name, new Palette(), data)
    {
    }
    public PaletteResource(string filepath) : base(filepath, new Palette())
    {
    }
    public static PaletteResource Create(string name, byte[] data)
    {
        return new(name, data);
    }
}
