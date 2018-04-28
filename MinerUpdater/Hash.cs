using System;
using System.IO;
using System.Security.Cryptography;

namespace MinerUpdater
{
    class Hash
    {
        public static string GetFileHash(string _targetFile)
        {
            if (!string.IsNullOrEmpty(_targetFile) && File.Exists(_targetFile))
            {
                using (var fileToHash = File.OpenRead(_targetFile))
                {
                    var hashAlgorithm = MD5.Create();
                    byte[] hashedFile = hashAlgorithm.ComputeHash(fileToHash);
                    string hashResult = BitConverter.ToString(hashedFile)
                                                    .Replace("-", string.Empty)
                                                    .ToLower();

                    return hashResult;
                }
            }
            else
            {
                return string.Empty;
            }
        }

        public static string GetLocalHash()
        {
            string localHashFile = AppConfig.Get("LocalHashFile");

            if (!string.IsNullOrEmpty(localHashFile) && File.Exists(localHashFile))
                return File.ReadAllText(localHashFile).ToLower();
            else
                return string.Empty;
        }

        public static string GetServerHash()
        {
            string serverHashUrl = AppConfig.Get("ServerHashUrl");
            string getServerHash = Web.GetTextAsString(serverHashUrl);

            if (!string.IsNullOrEmpty(getServerHash))
                return Web.GetTextAsString(serverHashUrl).ToLower();
            else
                return string.Empty;
        }

        public static bool IsHashMatch(string _hashOne, string _hashTwo)
        {
            if (!string.IsNullOrEmpty(_hashOne) && !string.IsNullOrEmpty(_hashTwo))
            {
                if (_hashOne == _hashTwo)
                    return true;
                else
                    return false;
            }
            else
            {
                return false;
            }     
        }
        
        public static bool UpdateLocalHash()
        {
            string localHashFile = AppConfig.Get("LocalHashFile");

            if (!string.IsNullOrEmpty(Hash.GetServerHash()))
            {
                using (var hashFile = new StreamWriter(localHashFile))
                {
                    hashFile.Write(Hash.GetServerHash());

                    Console.WriteLine(
                        Log.GetTimestamp()
                        + "Updated Local Hash to => "
                        + Hash.GetServerHash()
                    );

                    return true;
                }
            }
            else
            {
                return false;
            }      
        }
    }
}
