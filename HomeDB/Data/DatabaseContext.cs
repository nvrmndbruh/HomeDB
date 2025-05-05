using HomeDB.Models;
using SQLite;

namespace HomeDB.Data
{
    public class DatabaseContext
    {
        SQLiteAsyncConnection _database;

        public static string DbPath { get; } =
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "HomeDB.db");

        async Task Init()
        {
            if (_database != null)
                return;

            _database = new SQLiteAsyncConnection(DbPath);

            await _database.CreateTableAsync<Item>();
            await _database.CreateTableAsync<Container>();
            await _database.CreateTableAsync<Hierarchy>();
            await _database.CreateTableAsync<Category>();
            await _database.CreateTableAsync<ItemCategory>();
        }

        public async Task<IEnumerable<Item>> GetItems()
        {
            await Init();

            var items = await _database.Table<Item>().ToListAsync();
            return items;
        }

        public async Task<IEnumerable<Container>> GetContainers()
        {
            await Init();

            var containers = await _database.Table<Container>().ToListAsync();
            return containers;
        }

        public async Task<IEnumerable<Hierarchy>> GetHierarchies()
        {
            await Init();

            var hierarchies = await _database.Table<Hierarchy>().ToListAsync();
            return hierarchies;
        }
    }
}
