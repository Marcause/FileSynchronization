# Execution: 

`dotnet run -- <source_path>, <replica_path>, <log_path>, <interval>`

# Configuration:

Provide in the following order:
	- Source folder path
	- Replica folder path
	- Log file path
	- Synchronization interval in milliseconds

# Task:

Implement a program that synchronizes two folders: source and replica. The program should maintain a full, identical copy of source folder at replica folder.

- Synchronization must be one-way: after the synchronization content of the replica folder should be modified to exactly match the content of the source folder;
- Synchronization should be performed periodically.
- File creation/copying/removal operations should be logged to a file and to the console output;
- Folder paths, synchronization interval and log file path should be provided using the command line arguments;
- It is undesirable to use third-party libraries that implement folder synchronization;
- It is allowed (and recommended) to use external libraries implementing other well-known algorithms. For example, there is no point in implementing yet another function that calculates MD5 if you need it for the task – it is perfectly acceptable to use a third-party (or built-in) library.

# Solution explanation:

## Arguments

Arguments are checked before starting.
args[0] and [1] must be a valid Directory path.
args[2] must be a valid File path.
args[3] must be a valid int for the interval.

## Start

Before the loop and only once the arguments are recorded

## Program

The program is executing a loop every `interval` milliseconds with the function `syncDirectories()`

## Functions

- syncDirectories: writes start of sync time in log, calls the functions to copy and delete the files and then logs the time again.
- copyDirectory: copies all directories with Directory.CreateDirectory(), if the directory already exist, nothing happens.
Then it copies all the non-existing files and, if two files with the same name are found, checks the lastWriteDate (File.GetLastWriteTime).
If both dates are the same, nothing happens. This is done in order save time coping very long files.
If dates are different, the file will be overwritten.
Every change is logged.
- deleteDirectory: checks every directory and file and deletes the ones not present in the sourceFile but present in the replicaFile.
Every change is logged
- writeLog: a function made to avoid writing the same thing twice, it prints a given message in the console and in the logFile.
