using FormatReadLibrary.Logging.LoggingRegisters;
using LogRegister;
using System.Text.RegularExpressions;

namespace FormatReadLibrary.Readers
{
    public static class CommonValidations
    {
        public static bool ValidateEntryFormat(int i, string path, LogRegisterSystem log, string[] fileContentLines, Regex matcher, out Match m)
        {
            m = matcher.Match(fileContentLines[i]);
            if (!m.Success)
            {
                log.Add(new SyntaxError(path, i, fileContentLines[i], "Invalid Entry"));
                return false;
            }

            return true;
        }
        public static bool ValidateFileExists(string path, LogRegisterSystem log, string filepath)
        {
            if (!File.Exists(filepath))
            {
                log.Add(new ResourceNotFound(filepath));
                return false;
            }

            return true;
        }
        public static bool ValidateDuplicateID<T>(int i, string path, LogRegisterSystem log, string[] fileContentLines, int id, Dictionary<int, T> dictionary)
        {
            if (dictionary!.ContainsKey(id))
            {
                log.Add(new SyntaxError(path, i, fileContentLines[i], "Repeated ID"));
                return false;
            }

            return true;
        }
        public static bool ValidateEntryVariables(int i, string path, LogRegisterSystem log, string[] fileContentLines, out int[] values, Match m, bool allowVariables = false)
        {
            values = [];
            if (m!.Groups["var"].Success)
            {
                values = [..m.Groups["var"].Value
                    .Split(' ')
                    .Select(x => x[0] == '@' ?
                        int.Parse(x[1..]) :
                        Convert.ToInt32(x, 16))];
                if (values.Any(v => v < 0 || v > 255))
                {
                    log.Add(new SyntaxError(path, i, fileContentLines[i], "Variable values must be between 0 and 255 [00-FF]"));
                    return false;
                }
            }
            if (!allowVariables && values.Any())
            {
                log.Add(new SyntaxError(path, i, fileContentLines[i], "This list doesn't allow variable values"));
                return false;
            }

            return true;
        }
    }
}
