// See https://aka.ms/new-console-template for more information
Console.WriteLine("Provide Folder paths, synchronization interval and log file path");
Console.WriteLine(args.ToString());
Console.WriteLine(args.Length);

if (!(args.Length == 4))
{
    throw new ArgumentException("There should be 4 arguments");
}

string sourcePath = args[0];
string replicaPath = args[1];
string logPath = args[2];
int interval; //time in milliseconds

if (!Directory.Exists(sourcePath))
{
    throw new ArgumentException("1st argument should be a valid directory_path");
}

if (!Directory.Exists(replicaPath))
{
    throw new ArgumentException("2nd argument should be a valid directorty_path");
}

if (!File.Exists(logPath))
{
    throw new ArgumentException("3rd argument should be a valid file_path");
}

if (!int.TryParse(args[3], out interval))
{
    throw new ArgumentException("4th argument should be an int");
}

writeLog($@"
Starting with arguments
  - Source path: {sourcePath}
  - Replica path: {replicaPath}
  - Log path: {logPath}
  - Interval: {interval} ms
");

var timer = new PeriodicTimer(TimeSpan.FromMilliseconds(interval));

while (await timer.WaitForNextTickAsync())
{
    syncDirectories();
}

void syncDirectories()
{
    writeLog("Syncing starts at: "+DateTime.Now.ToString());
    copyDirectory(sourcePath, replicaPath);
    deleteDirectory(sourcePath, replicaPath);
    writeLog("Syncing completed at: " + DateTime.Now.ToString());
}

void copyDirectory(string source, string replica)
{
    var sourceDirs = Directory.GetDirectories(source, "*", SearchOption.AllDirectories);
    //Create all the subdirectories in the replica directory
    foreach (string dir in sourceDirs)
    {
        Directory.CreateDirectory(dir.Replace(source, replica));
    }

    var sourceFiles = Directory.GetFiles(source, "*.*", SearchOption.AllDirectories);
    //Copy all the files and Replaces any files with the same name
    foreach (string sourceFile in sourceFiles)
    {
        String replicaFile = sourceFile.Replace(source, replica);
        //checks if file exists in replica foulder
        if (File.Exists(replicaFile))
        {
            //checks if files are the same using date
            DateTime sourceTime = File.GetLastWriteTime(sourceFile);
            DateTime replicaTime = File.GetLastWriteTime(replicaFile);
            if (sourceTime != replicaTime)
            {
                String msn = "File is outdated, overwriting: " + sourceFile;
                writeLog(msn);
                File.Copy(sourceFile, replicaFile, true);
            }

        }
        else
        {
            String msn = "Coping file: " + sourceFile;

            writeLog(msn);
            File.Copy(sourceFile, replicaFile, true);
        }
    }
}

void deleteDirectory(string source, string replica)
{
    var dirSource = Directory.GetDirectories(source, "*", SearchOption.AllDirectories);
    var dirReplica = Directory.GetDirectories(replica, "*", SearchOption.AllDirectories);


    //deletes directories not in Source directory
    foreach (var directory in dirReplica.Reverse()) if (!Directory.Exists(directory.Replace(replica, source)))
        {
            writeLog("Deleting directory: " + directory);
            Directory.Delete(directory, true);
        }

    var filesSource = Directory.GetFiles(source, "*.*", SearchOption.AllDirectories);
    var filesReplica = Directory.GetFiles(replica, "*.*", SearchOption.AllDirectories);

    //deletes files not in Source directory
    foreach (var file in filesReplica) if (!File.Exists(file.Replace(replica, source)))
        {
            writeLog("Deleting file: " + file);
            File.Delete(file);
        }
}

void writeLog(String msn)
{
    String message = msn + "\n";
    File.AppendAllText(logPath, message);
    Console.WriteLine(msn);
}