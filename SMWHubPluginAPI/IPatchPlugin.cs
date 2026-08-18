using System.Text;

namespace SMWHubPluginAPI;

public interface IPatchPlugin
{
    public StringBuilder BuildPatch(InstallationContext context, IPackage package);
    public string Insert(InstallationContext context, StringBuilder patchContent);
}
