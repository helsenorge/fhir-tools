/* 
 * Copyright (c) 2021 - 2023, Norsk Helsenett SF and contributors
 * See the file CONTRIBUTORS for details.
 * 
 * This file is licensed under the MIT license
 * available at https://raw.githubusercontent.com/helsenorge/fhir-tools/master/LICENSE
 */

using EnsureThat;
using System.IO;

namespace FhirTool.Core
{
    public static class IOUtility
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
