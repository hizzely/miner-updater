using System;
using System.Diagnostics;

namespace MinerUpdater
{
    class ProcessMan
    {
        static readonly Process process = new Process();

        string _processName;
        string _processArgs;

        public string ProcessName
        {
            get
            {
                return _processName;
            }
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    throw new ArgumentException("Make sure your input is valid.");
                }
                else
                {
                    _processName = value;
                }
            }
        }

        public string ProcessArgs
        {
            get
            {
                return _processArgs;
            }
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    _processArgs = string.Empty;
                }
                else
                {
                    _processArgs = value;
                }
            }
        }

        public ProcessMan(string processPath, string processArg)
        {
            ProcessName = processPath;
            ProcessArgs = processArg;
        }


        public bool StartProcess(bool _waitToClose = false)
        {
            Console.WriteLine(
                Log.GetTimestamp()
                    + "Start Process: "
                    + _processName
                    + " "
                    + _processArgs
            );

            try
            {
                var processToRun = new ProcessStartInfo(_processName, _processArgs);
                process.StartInfo = processToRun;

                if (process.Start())
                {
                    if (_waitToClose)
                    {
                        process.WaitForExit();
                        if (process.ExitCode == 0)
                        {
                            return true;
                        }
                        else
                        {
                            return false;
                        }   
                    }
                        
                    return true;
                }
                else
                {
                    return false;
                }    
            }
            catch (Exception ex)
            {
                Console.WriteLine(
                    Log.GetTimestamp()
                    + "Start Process: "
                    + ex.Message
                );

                Log.ExceptionExport(
                    Log.GetTimestamp()
                    + "Start Process: "
                    + ex.ToString()
                );

                return false;
            }
        }
    }
}
