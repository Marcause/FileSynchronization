// See https://aka.ms/new-console-template for more information
Console.WriteLine("Hello, World!");

string sourcePath = "C:\\Users\\Marcos\\Desktop\\FileSynch\\Original";
string replicaPath = "C:\\Users\\Marcos\\Desktop\\FileSynch\\Copied";
string logPath = "C:\\Users\\Marcos\\Desktop\\FileSynch\\Log\\log.txt";
float interval = 2; //time in seconds

var timer = new PeriodicTimer(TimeSpan.FromSeconds(interval));

while (await timer.WaitForNextTickAsync())
{

    Console.WriteLine("*****");
    copyDirectory(sourcePath, replicaPath);
    deleteDirectory(sourcePath, replicaPath);

}

void copyDirectory(string source, string replica)
{
    //Create all the subdirectories in the replica directory
    foreach (string dirPath in Directory.GetDirectories(source, "*", SearchOption.AllDirectories))
    {
        Directory.CreateDirectory(dirPath.Replace(source, replica));
    }

    //Copy all the files and Replaces any files with the same name
    foreach (string newPath in Directory.GetFiles(source, "*.*", SearchOption.AllDirectories))
    {
        Console.WriteLine("File: " + newPath.Split('\\').Last() + " is being copied in: " + newPath.Replace(source, replica));
        File.Copy(newPath, newPath.Replace(source, replica), true);
    }

}

void deleteDirectory(string source, string replica)
{
    var dirSource = Directory.GetDirectories(source, "*", SearchOption.AllDirectories);
    var dirReplica = Directory.GetDirectories(replica, "*", SearchOption.AllDirectories);

    //deletes directories not in Source directory
    foreach (var directory in dirReplica) if (!Directory.Exists(Path.Combine(source, directory.Split('\\').Last())))
        {
            Console.WriteLine("Deleting directory: " + directory);
            Directory.Delete(directory, true);

        }

    var filesSource = Directory.GetFiles(source, "*.*", SearchOption.AllDirectories);
    var filesReplica = Directory.GetFiles(replica, "*.*", SearchOption.AllDirectories);

    //deletes files not in Source directory
    foreach (var files in filesReplica) if (!File.Exists(Path.Combine(source, files.Split('\\').Last())))
        {
            Console.WriteLine("Deleting file: " + files);
            File.Delete(files);

        }

    /*        foreach(var directory in dirReplica) if (!Directory.Exists(Path.Combine(source, directory.Split('\\').Last())))
            {

               // Console.WriteLine(Path.Combine(replica, directory.Split('\\').Last()));
                //Console.WriteLine(Path.Combine(source, directory.Split('\\').Last()));
                Console.WriteLine(Directory.Exists(Path.Combine(source, directory.Split('\\').Last())));
                Console.WriteLine(directory);
                Directory.Delete(directory, true);
                Console.WriteLine("Deleting directory: " + directory);
            }*/
}