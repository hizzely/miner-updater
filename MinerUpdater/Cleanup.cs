using System;
using System.IO;
using System.Collections.Generic;

namespace MinerUpdater
{
    class Cleanup
    {
        public static void PreserveLatest(string _targetDir, int _maxCycle)
        {
            if (!string.IsNullOrEmpty(_targetDir))
            {
                List<string> listFiles = new List<string>();

                foreach (string _f in Directory.GetFiles(_targetDir))
                {
                    listFiles.Add(_f);
                }

                if (listFiles.Count > _maxCycle)
                {
                    listFiles.Sort();
                    listFiles.Reverse();

                    for (int x = listFiles.Count; x > _maxCycle; x--)
                    {
                        int y = x - 1;
                        FileFolder.DeleteFile(listFiles[y]);
                        Console.WriteLine(
                            Log.GetTimestamp()
                            + "Deleted: "
                            + listFiles[y]
                        );
                    }
                }
            }
            else
            {
                Console.WriteLine(
                    Log.GetTimestamp()
                    + "Cleanup Error! Make sure target is exist!"
                );
            }
        }
    }
}
