using SMWHubASMCodeLibrary;
using SMWHubInstallation.DTO;
using Validations;

namespace SMWHubInstallation
{
    public class Installer(string configPath)
    {
        private readonly PathContainerDTO _paths = PathContainerDTO.FromJson(File.ReadAllText(configPath));
        public ValidationResult Install()
        {
            ValidationResult validation = new();
            List<Code> codes = [];

            SharedCodePathProcessor scpp = new(_paths.FolderConfigPath);

            codes.AddRange(scpp.FindSharedCodes());
            return validation;
        }
        private static void installSprites(IEnumerable<Code> codes)
        {

        }
    }
}
