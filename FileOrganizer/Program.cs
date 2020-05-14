using System;
using System.IO;
using System.Security.Permissions;
using System.Threading;

public class Watcher
{
    public static void Main()
    {
        Run();
    }

    [PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
    private static void Run()
    {
        string[] args = Environment.GetCommandLineArgs();

        // If a directory is not specified, exit program.
        if (args.Length != 2)
        {
            // Display the proper way to call the program.
            Console.WriteLine("Usage: Watcher.exe (directory)");
            return;
        }

        // Create a new FileSystemWatcher and set its properties.
        using (FileSystemWatcher watcher = new FileSystemWatcher())
        {
            watcher.Path = args[1];
            Console.WriteLine($"+|| Listening to files in {args[1]} ||+");

            // Watch for changes in LastAccess and LastWrite times, and
            // the renaming of files or directories.
            watcher.NotifyFilter = 
                                 //  NotifyFilters.LastAccess
                                 //| NotifyFilters.LastWrite
                                 //| 
                                 NotifyFilters.FileName
                                 | NotifyFilters.DirectoryName;

            // Only watch text files.
            watcher.Filter = "*.pdf";

            // Add event handlers.
            //watcher.Changed += OnChanged;
            watcher.Created += OnChanged;
            //watcher.Deleted += OnChanged;
            //watcher.Renamed += OnRenamed;

            // Begin watching.
            watcher.EnableRaisingEvents = true;

            // Wait for the user to quit the program.
            Console.WriteLine("Press 'q' to stop listening.");
            while (Console.Read() != 'q') ;
        }
    }

    private static void HandleFileCreated(string path, string fileName, System.IO.WatcherChangeTypes changeType){
        if(changeType == WatcherChangeTypes.Created)
        {
            Thread.Sleep(3000);
            try
            {   
                if (!File.Exists(path))
                {
                    //    This statement ensures that the file is created,
                    //     but the handle is not kept.
                    using (FileStream fs = File.Create(path)) { }
                }

                string[] args = Environment.GetCommandLineArgs();

                string baseFolder = args[1];

                string year = DateTime.Now.Year.ToString();
                string[] monthNames = { "Janurary", "Feburary", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "September", "October", "November", "December" };
                string monthName = monthNames[DateTime.Now.Month - 1];
                string dayOfMonth = DateTime.Now.Day.ToString();

                string datePath = System.IO.Path.Combine(baseFolder, year, monthName, dayOfMonth);
                if (!Directory.Exists(datePath))
                {
                    System.IO.Directory.CreateDirectory(datePath);
                }

                string destPath = System.IO.Path.Combine(datePath, fileName);

                // Ensure that the target does not exist.
                if (File.Exists(destPath))
                    File.Delete(destPath);

                // Move the file.
                Thread.Sleep(2000);
                File.Move(path, destPath, true);
                Console.WriteLine("{0} was moved to {1}.", path, destPath);

                // See if the original exists now.
                if (File.Exists(path))
                {
                    Console.WriteLine("The original file still exists, which is unexpected.");
                }
                else
                {
                    Console.WriteLine("The original file no longer exists, which is expected.");
                }

            }
            catch (Exception e)
            {
                Console.WriteLine("The process failed: {0}", e.ToString());
            }
        };
    }

    // Define the event handlers.
    private static void OnChanged(object source, FileSystemEventArgs e) =>
        // Specify what is done when a file is changed, created, or deleted.
        HandleFileCreated(e.FullPath, e.Name, e.ChangeType);

    private static void OnRenamed(object source, RenamedEventArgs e) =>
        // Specify what is done when a file is renamed.
        Console.WriteLine($"File: {e.OldFullPath} renamed to {e.FullPath}");
}
