using EnsureThat;
using System.IO;

namespace FhirTool.Core
{
    public static class IOHelpers
    {
        private static string FileNameReservedCharacters = "<>:\"/\\|?*";

        public static bool IsDirectory(string path)
        {
            EnsureArg.IsNotNullOrWhiteSpace(path, nameof(path));

            FileAttributes attr = File.GetAttributes(path);
            return (attr & FileAttributes.Directory) == FileAttributes.Directory;
        }
        
        public static string GenerateLegalFilename(string path)
        {
            string legalFilename = path;
            foreach (char c in FileNameReservedCharacters)
            {
                legalFilename = legalFilename.Replace(c, '_');
            }

            return legalFilename;
        }
    }
}
