using EnsureThat;
using System.IO;

namespace FhirTool
{
    internal static class IOHelpers
    {
        internal static bool IsDirectory(string path)
        {
            EnsureArg.IsNotNullOrWhiteSpace(path, nameof(path));

            FileAttributes attr = File.GetAttributes(path);
            return (attr & FileAttributes.Directory) == FileAttributes.Directory;
        }
    }
}
