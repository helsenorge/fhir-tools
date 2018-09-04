using System.IO;

namespace FhirTool
{
    internal static class IOHelpers
    {
        internal static bool IsDirectory(string path)
        {
            FileAttributes attr = File.GetAttributes(path);
            return (attr & FileAttributes.Directory) == FileAttributes.Directory;
        }
    }
}
