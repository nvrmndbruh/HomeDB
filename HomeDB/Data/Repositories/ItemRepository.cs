using HomeDB.Data.Repositories.Interfaces;
using HomeDB.Models;
using SQLite;

namespace HomeDB.Data.Repositories
{
    public class ItemRepository : IRepository<Item>
    {
        private SQLiteAsyncConnection _database;

        public async Task Init()
        {
            if (_database != null)
                return;

            _database = new SQLiteAsyncConnection(DatabaseContext.DbPath, SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.Create | SQLiteOpenFlags.SharedCache);

            await _database.CreateTableAsync<Item>();
        }

        public async Task<Item> GetAsync(int id)
        {
            await Init();

            return await _database.Table<Item>().FirstOrDefaultAsync(i => i.Id == id);
        }
        public async Task<IEnumerable<Item>> GetAllAsync()
        {
            await Init();

            return await _database.Table<Item>().ToListAsync();
        }

        public async Task InsertAsync(Item entity)
        {
            await Init();

            await _database.InsertAsync(entity);
        }

        public async Task UpdateAsync(Item entity)
        {
            await Init();

            await _database.UpdateAsync(entity);
        }

        public async Task DeleteAsync(int id)
        {
            await Init();

            await _database.DeleteAsync<Item>(id);
        }
    }
}
