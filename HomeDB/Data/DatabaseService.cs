using HomeDB.Model;
using HomeDB.Models;
using SQLite;

namespace HomeDB.Data
{
    public class DatabaseService
    {
        private SQLiteAsyncConnection _database;

        public async Task Init()
        {
            if (_database is not null)
                return;

            _database = new SQLiteAsyncConnection(Constants.DatabasePath, Constants.Flags);
            var result = await _database.CreateTableAsync<Item>()
                & await _database.CreateTableAsync<Container>()
                & await _database.CreateTableAsync<Hierarchy>()
                & await _database.CreateTableAsync<Category>()
                & await _database.CreateTableAsync<ItemCategory>();
        }


    }
}