using System.Reflection;

namespace HomeDB.Models
{
    public class DatabaseInitializer
    {
        public static void Initialize()
        {
            var dbPath = Path.Combine(FileSystem.AppDataDirectory, "HomeDB.db");
            if (!File.Exists(dbPath))
            {
                using var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("HomeDB.Resources.Raw.HomeDB.db");
                using var fileStream = File.Create(dbPath);
                stream.CopyTo(fileStream);
            }
        }
    }
}
