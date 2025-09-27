using HomeDB.Models;
using SQLite;

namespace HomeDB.Data
{
    public class DatabaseContext
    {
        private const string databaseName = "HomeDB.db";

        public static string DbPath { get; } =
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), databaseName);
    }
}
