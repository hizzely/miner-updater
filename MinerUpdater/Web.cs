using System.Net;

namespace MinerUpdater
{
    class Web
    {
        static readonly WebClient http = new WebClient();

        public static string GetTextAsString(string _textUrl)
        {
            if (!string.IsNullOrEmpty(_textUrl))
            {
                try
                {
                    return http.DownloadString(_textUrl);
                }
                catch (WebException ex)
                {
                    System.Console.WriteLine(
                        Log.GetTimestamp()
                        + ex.Message
                    );

                    return string.Empty;
                }
            }
            else
            {
                return string.Empty;
            }
        }

        public static bool DownloadFile(string _fileUrl, string _saveTo)
        {
            if (!string.IsNullOrEmpty(_fileUrl))
            {
                try
                {
                    http.DownloadFile(_fileUrl, _saveTo);

                    return true;
                }
                catch (WebException ex)
                {
                    System.Console.WriteLine(
                        Log.GetTimestamp()
                        + ex.Message
                    );

                    return false;
                }
            }
            else
            {
                return false;
            }
        }
    }
}
