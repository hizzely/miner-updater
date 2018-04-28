using System;
using System.IO;
using System.IO.Compression;

namespace MinerUpdater
{
    class Archive
    {
        public static bool FromDirectory(
            string _sourceDir, string _saveTo, bool _inclParentFolder = false)
        {
            if (!string.IsNullOrEmpty(_sourceDir) && Directory.Exists(_sourceDir))
            {
                ZipFile.CreateFromDirectory(
                    _sourceDir,
                    _saveTo,
                    CompressionLevel.Fastest,
                    _inclParentFolder
                );

                return true;
            }
            else
            {
                return false;
            }   
        }

        public static bool ExtractTo(string _archiveFile, string _targetDir)
        {
            if (!string.IsNullOrEmpty(_archiveFile) && File.Exists(_archiveFile))
            {
                if (!Directory.Exists(_targetDir))
                    FileFolder.CreateFolder(_targetDir);

                ZipFile.ExtractToDirectory(_archiveFile, _targetDir);

                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
