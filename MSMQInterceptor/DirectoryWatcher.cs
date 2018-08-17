using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Permissions;
using System.Text;
using System.Threading.Tasks;

namespace MSMQInterceptor
{    
    class DirectoryWatcher
    {
        [PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
        public static void Run()
        {
            FileSystemWatcher watcher = new FileSystemWatcher
            {
                Path = Directory.GetCurrentDirectory() + @"\XMLFiles\",

                NotifyFilter = NotifyFilters.LastAccess | NotifyFilters.LastWrite | NotifyFilters.FileName | NotifyFilters.DirectoryName,

                Filter = "*.xml"
            };

            //watcher.Changed += new FileSystemEventHandler(OnChanged);
            watcher.Created += new FileSystemEventHandler(OnChanged);
            //watcher.Deleted += new FileSystemEventHandler(OnChanged);

            watcher.EnableRaisingEvents = true;

            Console.WriteLine("Press \'q\' to quit the sample.");
            while (Console.Read() != 'q');
        }

        private static void OnChanged(object source, FileSystemEventArgs e)
        {
            // Specify what is done when a file is changed, created, or deleted.
           Console.WriteLine("File: " +  e.FullPath + " " + e.ChangeType);
           Program.SendMessages(e.FullPath);
           MSMQHelper.ProcessMessageQueue();
        }
    }
}
