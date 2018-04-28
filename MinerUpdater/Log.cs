using System;
using System.IO;

namespace MinerUpdater
{
    class Log
    {
        public static string GetTimestamp()
        {
            return "[" + DateTime.Now.ToString() + "] ";
        }

        public static string GetFileTimestamp()
        {
            return DateTime.Now
                .ToString()
                .Replace("/", string.Empty)
                .Replace(" ", "-")
                .Replace(":", string.Empty);
        }

        public static bool ExceptionExport(string _trace)
        {
            string exceptionLogFile = AppConfig.Get("ExceptionLogFile");

            using (var exportToFile = new StreamWriter(exceptionLogFile, true))
            {
                exportToFile.Write(_trace);

                return true;
            }
        }
    }
}
