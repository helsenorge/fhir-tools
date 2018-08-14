using System.IO;

namespace FhirTool
{
    public class Logger
    {
        private static TextWriter _out = null;
        private static bool _verbose = false;

        public static void Initialize(TextWriter out_, bool verbose = false)
        {
            _out = out_;
            _verbose = verbose;
        }

        public static void DebugWriteLineToOutput(string value)
        {
            if (_verbose)
                _out.WriteLine($"DEBUG: {value}");
        }

        public static void InfoWriteLineToOutput(string value)
        {
            _out.WriteLine($"INFO: {value}");
        }

        public static void ErrorWriteLineToOutput(string value)
        {
            _out.WriteLine($"ERROR: {value}");
        }

        public static void WarnWriteLineToOutput(string value)
        {
            _out.WriteLine($"WARN: {value}");
        }

        public static void WriteLineToOutput(string value = "")
        {
            _out.WriteLine(value);
        }
    }
}
