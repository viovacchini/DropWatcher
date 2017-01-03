using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Security.Permissions;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace DropWatcher
{
	class Program
	{
		internal static object watchLocker = new object();
		internal static ModifiedFileList modifiedFileList = new ModifiedFileList();
		internal static string filterRegEx;
		internal static string sourceDirectory;
		internal static string targetDirectory;

		static void Main( string [] args )
		{

			try
			{
				// Create a new FileSystemWatcher and set its properties.
				FileSystemWatcher watcher = new FileSystemWatcher();
				sourceDirectory = ConfigurationManager.AppSettings ["sourceDirectory"];
				targetDirectory = ConfigurationManager.AppSettings ["targetDirectory"];

				watcher.Path = sourceDirectory;
				/* Watch for changes in LastAccess and LastWrite times, and
					 the renaming of files or directories. */
				watcher.NotifyFilter = NotifyFilters.LastWrite;
				// Only watch text files.
				filterRegEx = ConfigurationManager.AppSettings["filterRegEx"];
				watcher.Filter = "*.*";

				// Add event handlers.
				watcher.Changed += new FileSystemEventHandler( OnChanged );

				// Begin watching.
				watcher.EnableRaisingEvents = true;

				// Wait for the user to quit the program.
				Console.WriteLine( "Source directory: {0} \nTarget Directory: {1} \nFilterRegEx: {2}", 
					sourceDirectory, targetDirectory, filterRegEx );
				Console.WriteLine( "Press \'q\' to quit." );
				while ( Console.Read() != 'q' ) ;

			}
			catch ( Exception ex )
			{
				Console.Write( ex.Message );
			}
		}

		private static void OnChanged(object source, FileSystemEventArgs e)
		{
			lock ( watchLocker )
			{

				string strFileExt = Path.GetExtension( e.FullPath ); 

				// filter file types 
				if (Regex.IsMatch(strFileExt, filterRegEx, RegexOptions.IgnoreCase)) 
				{ 
						Console.WriteLine("watched file type changed."); 
				} 

				FileInfo fi = new FileInfo( e.FullPath );
				if ( modifiedFileList.ContainsModifiedFile(fi) )
				{
					// The file is already being processed; just exit
					return;
				}

				modifiedFileList.Add( fi );
				Console.WriteLine("File: " + e.FullPath + " " + e.ChangeType + "\r\n" + e.ToString() );

				File.Move( e.FullPath, e.FullPath.Replace(sourceDirectory, targetDirectory) );
				modifiedFileList.CheckForCleanup();
			}

		}
	}
}
