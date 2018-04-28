# Miner Updater
A simple tool for automating miner software update.

## Background
I made this Updater app as my first project on learning C# and for making easy to update a mining application that installed on multiple machines.
But it should be work on any other applications as long as your application only need to update things inside it's own directory to be be work.
Because that's how this Updater works in handling update (based on NiceHashMinerLegacy updating process).

## How to use
1. Get a machine with web server installed for storing and serving the update file and hash file.
2. If you have Visual Studio, you can clone this repository and build the app.  
If you don't you can download the 'ready' version and extract it somewhere.
3. Customize `MinerUpdater.exe.config` to fit your setup.  
Explanation on what are those key mean inside the file [can be found here](https://github.com/hizzely/miner-updater/wiki/Config-File-Explanation).
4. Automate the Updater run schedule by using Task Scheduler on Windows.  

## License
Copyright (C) 2018 Fajar Ru. Licensed under MIT.