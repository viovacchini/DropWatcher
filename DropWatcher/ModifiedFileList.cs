using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DropWatcher
{
	public class ModifiedFileList : List< FileInfo >
	{
		private const int MAX_BUFFER_SIZE = 5;
		private const int CLEANUP_SIZE = 2;

		public bool ContainsModifiedFile( FileInfo fi )
		{
			return this.Any( 
				mf => mf.FullName == fi.FullName && mf.LastWriteTime == fi.LastWriteTime );
		}

		public void CheckForCleanup()
		{
			if ( this.Count > MAX_BUFFER_SIZE	)
			{
				this.RemoveRange( 0, CLEANUP_SIZE );
				Console.WriteLine( "Removed {0} items from the ModifiedFileList", CLEANUP_SIZE );
			}
		}
	}
}
