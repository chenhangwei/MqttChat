using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chat.Constants
{
  public static class Constants
    {
        public const string DatabaseFilename = "TodoSQLite.db3";//C:\Users\chenh\AppData\Local\Packages\com.companyname.chat_9zz4h110yvjzm\LocalState
        public const SQLite.SQLiteOpenFlags Flags =
            SQLite.SQLiteOpenFlags.ReadWrite |
            SQLite.SQLiteOpenFlags.Create |
            SQLite.SQLiteOpenFlags.SharedCache;
        public static string DatabasePath =>
            Path.Combine(FileSystem.AppDataDirectory, DatabaseFilename);
    }
}
