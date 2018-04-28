using System;
using System.IO;

namespace MinerUpdater
{
    class FileFolder
    {
        public static bool CreateFolder(string _target)
        {
            if (!string.IsNullOrEmpty(_target) && !Directory.Exists(_target))
            {
                Directory.CreateDirectory(_target);

                return true;
            }
            else
            {
                return false;
            }    
        }

        public static bool DeleteFolder(string _targetFolder)
        {
            if (!string.IsNullOrEmpty(_targetFolder) && Directory.Exists(_targetFolder))
            {
                Directory.Delete(_targetFolder, true);
                
                return true;
            }
            else
            {
                return false;
            }
        }

        public static bool DeleteFile(string _targetFile)
        {
            if (!string.IsNullOrEmpty(_targetFile) && File.Exists(_targetFile))
            {
                File.Delete(_targetFile);

                return true;
            }
            else
            {
                return false;
            }
        }

        public static bool MoveFolder(string _oldFolder, string _newFolder)
        {
            if (!string.IsNullOrEmpty(_oldFolder) && !string.IsNullOrEmpty(_newFolder))
            {
                try
                {
                    Directory.Move(_oldFolder, _newFolder);

                    return true;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    return false;
                }   
            }
            else
            {
                return false;
            }
        }

        public static void SwapDirectory(string _oldDirName, string _newDirName)
        {
            if (Directory.Exists(_oldDirName))
            {
                if (FileFolder.DeleteFolder(_oldDirName))
                {
                    Console.WriteLine(
                        Log.GetTimestamp()
                        + "Deleted: "
                        + _oldDirName
                    );
                } 
            }
                
            if (Directory.Exists(_newDirName))
            {
                if (FileFolder.MoveFolder(_newDirName, _oldDirName))
                {
                    Console.WriteLine(
                        Log.GetTimestamp()
                        + "Renamed: "
                        + _newDirName
                        + " => "
                        + _oldDirName
                    );
                }
            }
        }
    }
}
