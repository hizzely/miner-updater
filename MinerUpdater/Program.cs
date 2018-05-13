using System;
using System.IO;
using System.Threading;

namespace MinerUpdater
{
    class Program
    {
        static void Main()
        {
            // -- Load App Configuration
            string programName = AppConfig.Get("ProgramName");
            string isFirstTime = AppConfig.Get("IsFirstTime");
            string appFolder = AppConfig.Get("AppFolder");
            string appUpdateTempExtractDir = AppConfig.Get("AppUpdateTempExtractDir");

            Console.Title = programName;

            // First Log Output
            Console.WriteLine(
                Log.GetTimestamp() 
                + programName 
                + " started \n"
            );


            // Run preexecution script
            if (!File.Exists(AppConfig.Get("LocalHashFile")))
            {
                Console.WriteLine(
                    Log.GetTimestamp()
                    + "Local Hash not found, assuming it's a first time run."
                );
            }
            else
            {
                string preExecAppPath = AppConfig.Get("PreExecAppPath");
                string preExecAppArgs = AppConfig.Get("PreExecAppArgs");

                var launchPXS = new ProcessMan(preExecAppPath, preExecAppArgs).StartProcess(true);

                if (AppConfig.Get("PreExecExpectReturnZero") == "true")
                {
                    Console.WriteLine(
                        Log.GetTimestamp()
                        + "PXS running with expectation of return error code 0."
                    );

                    if (!launchPXS)
                    {
                        Console.WriteLine(
                            Log.GetTimestamp()
                            + "Error! Your PXS script did not return the expected value."
                        );
                        System.Windows.Forms.MessageBox.Show(
                            "Error! Your PXS script did not return the expected value. \n"
                            + "Check your PXS script and restart the updater program."
                            + "If you want the updater to ignore any returned value, "
                            + "change PreExecExpectReturnZero to false.",
                            programName + ": PreExecutionScript Error",
                            System.Windows.Forms.MessageBoxButtons.OK,
                            System.Windows.Forms.MessageBoxIcon.Error
                        );

                        return;
                    }
                    else
                    {
                        Console.WriteLine(
                            Log.GetTimestamp()
                            + "Success! PXS returned the expected value."
                        );
                    }
                }
                else
                {
                    Console.WriteLine(
                        Log.GetTimestamp()
                        + "Warning! PXS running without any return expectation."
                    );
                }
            }


            // Run Update Checking
            if (!Hash.IsHashMatch(Hash.GetLocalHash(), Hash.GetServerHash()))
            {
                Console.WriteLine(
                    Log.GetTimestamp()
                    + "Hash not match, Preparing to update...\n"
                );

                // 1. Backup old app folder (if exists)
                if (Directory.Exists(appFolder))
                {
                    string appBackupDirectory = AppConfig.Get("AppBackupDirectory");
                    string appBackupNamePrefix = AppConfig.Get("AppBackupNamePrefix");

                    string backupName = 
                        appBackupNamePrefix
                        + appFolder.Replace("/", string.Empty)
                        + "_"
                        + Log.GetFileTimestamp()
                        + ".zip";

                    if (!Directory.Exists(appBackupDirectory))
                    {
                        bool createFolder = FileFolder.CreateFolder(appBackupDirectory);

                        if (createFolder)
                        {
                            Console.WriteLine(
                                Log.GetTimestamp()
                                + "Created backup directory: "
                                + appBackupDirectory
                            );
                        }
                    }

                    var doBackup = Archive.FromDirectory(
                        appFolder, 
                        appBackupDirectory + backupName, 
                        true
                    );

                    if (doBackup)
                    {
                        Console.WriteLine(
                            Log.GetTimestamp()
                            + "Backup created: "
                            + appBackupDirectory
                            + backupName
                        );
                    }
                }


                // 2. Old Downloaded Update Cleanup & Download New Update File
                string appUpdateFileName = AppConfig.Get("AppUpdateFileName");
                string appUpdateFolderName = AppConfig.Get("AppUpdateFolderName");
                string serverUpdateFileUrl = AppConfig.Get("ServerUpdateFileUrl");
                string getUpdateFilePath = appUpdateFolderName + appUpdateFileName;

                if (Directory.Exists(appUpdateFolderName))
                {
                    bool deleteFolder = FileFolder.DeleteFolder(appUpdateFolderName);

                    if (deleteFolder)
                    {
                        Console.WriteLine(
                            Log.GetTimestamp()
                            + "Deleted old update directory: "
                            + appUpdateFolderName
                        );
                    }  
                }
                    
                if (!Directory.Exists(appUpdateFolderName))
                {
                    bool createFolder = FileFolder.CreateFolder(appUpdateFolderName);

                    if (createFolder)
                    {
                        Console.WriteLine(
                            Log.GetTimestamp()
                            + "Created update directory: "
                            + appUpdateFolderName
                        );
                    }
                }

                do
                {
                    Console.WriteLine(
                        Log.GetTimestamp() 
                        + "Downloading: " 
                        + AppConfig.Get("ServerUpdateFileUrl")
                    );

                    bool downloadFile = Web.DownloadFile(
                        serverUpdateFileUrl,
                        getUpdateFilePath
                    );

                    if (downloadFile)
                    {
                        Console.WriteLine(
                            Log.GetTimestamp()
                            + "Download Complete => "
                            + getUpdateFilePath
                        );
                    }

                    Hash.UpdateLocalHash();                        
                } while (
                    !Hash.IsHashMatch(
                        Hash.GetFileHash(getUpdateFilePath), 
                        Hash.GetServerHash()
                  )
                );


                // 3. Extract Downloaded Update Into Temporary Folder
                if (Directory.Exists(appUpdateTempExtractDir))
                {
                    FileFolder.DeleteFolder(appUpdateTempExtractDir);

                    Console.WriteLine(
                        Log.GetTimestamp()
                        + "Deleted old extraction folder: "
                        + appUpdateTempExtractDir
                    );
                }
                    
                bool extractFile = Archive.ExtractTo(
                    getUpdateFilePath, 
                    appUpdateTempExtractDir
                );

                if (extractFile)
                {
                    Console.WriteLine(
                        Log.GetTimestamp()
                        + "Extracted: "
                        + getUpdateFilePath
                        + " => "
                        + appUpdateTempExtractDir
                    );
                }


                // 4. Move User-specific configs data to the New Updated App
                string[] appUserSpecificData = AppConfig.Get("AppUserSpecificData").Split(',');

                for (int x = 0; x < appUserSpecificData.Length; x++)
                {
                    if (Directory.Exists(appFolder + appUserSpecificData[x]))
                    {
                        bool moveToNewFolder = FileFolder.MoveFolder(
                            appFolder + appUserSpecificData[x], 
                            appUpdateTempExtractDir + appUserSpecificData[x]
                        );

                        if (moveToNewFolder)
                        {
                            Console.WriteLine(
                                Log.GetTimestamp()
                                + "Moved: "
                                + appFolder
                                + appUserSpecificData[x]
                                + " => "
                                + appUpdateTempExtractDir
                                + appUserSpecificData[x]);
                        }    
                    }       
                }


                // 5. Swap Directories. 
                string appDirName = appFolder;
                string tempDirName = appUpdateTempExtractDir;

                if(FileFolder.DeleteFolder(appDirName))
                {
                    Console.WriteLine(
                        Log.GetTimestamp()
                        + "Deleted: "
                        + appDirName
                    );
                }
                    
                if(FileFolder.MoveFolder(tempDirName, appDirName))
                {
                    Console.WriteLine(
                        Log.GetTimestamp()
                        + "Moved: "
                        + tempDirName
                        + " => "
                        + appDirName);
                }
            }


            // 6. Old files cleanup
            if (Directory.Exists(AppConfig.Get("AppBackupDirectory")))
            {
                Console.WriteLine(Log.GetTimestamp() + "Cleaning: Old Backup Files");
                Cleanup.PreserveLatest(
                    AppConfig.Get("AppBackupDirectory"),
                    int.Parse(AppConfig.Get("AppBackupPreserveFileCycles"))
                );
            }
            
            if (Directory.Exists(AppConfig.Get("AppUpdateFolderName")))
            {
                Console.WriteLine(Log.GetTimestamp() + "Cleaning: Old Update Files");
                Cleanup.PreserveLatest(
                    AppConfig.Get("AppUpdateFolderName"),
                    int.Parse(AppConfig.Get("AppUpdatePreserveFileCycles"))
                );
            }

            
            // 7. Finally run the actual application.
            Console.WriteLine(
                "\n" + Log.GetTimestamp() + "Hash Match. "
                + "will continue running your program."
            );

            string launchProgram = AppConfig.Get("LaunchProgram");
            string launchProgramArgs = AppConfig.Get("LaunchProgramArgs");

            var launchApp = new ProcessMan(launchProgram, launchProgramArgs);

            if (!launchApp.StartProcess())
            {
                Console.WriteLine(
                    Log.GetTimestamp()
                    + "Error when trying to launch your program"
                );
            }


            // End of this app jobs. 
            // Close the app and wait the scheduler to call it again.
            // and everything start over again.
            Console.WriteLine(
                "\n" 
                + Log.GetTimestamp()
                + programName
                + " jobs is over. Closing in 30 secs..."
            );

            Thread.Sleep(30000);

            return;
        }
    }
}
