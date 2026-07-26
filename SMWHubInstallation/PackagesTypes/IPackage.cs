using SMWHubASMCodeLibrary;
using System.Security.Cryptography;

namespace SMWHubInstallation.PackagesTypes
{
    public interface IPackage
    {
        public CodeScope Scope { get; }
        public string PackagePath { get; }
        public IPackage? GetSubPackageFromInternalFile(string filepath);
        public IEnumerable<string> GetFiles();
        public long GetSize();
        public string ObtainHashCode();
        public static string GetHashFromFile(string path)
        {
            using FileStream stream = File.OpenRead(path);
            using SHA512 sha512 = SHA512.Create();

            byte[] hash = sha512.ComputeHash(stream);

            return Convert.ToBase64String(hash);
        }
        public static string GetHashFromMultiplesFiles(IEnumerable<string> paths)
        {
            List<byte> hashes = [];

            foreach (string path in paths.Order())
            {
                using FileStream stream = File.OpenRead(path);
                using SHA512 sha5121 = SHA512.Create();

                hashes.AddRange(sha5121.ComputeHash(stream));
            }
            using SHA512 sha5122 = SHA512.Create();
            byte[] hash = sha5122.ComputeHash([.. hashes]);
            return Convert.ToBase64String(hash);
        }
    }
}
