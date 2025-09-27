using HomeDB.Data.Repositories.Interfaces;
using HomeDB.Models;
using SQLite;

namespace HomeDB.Data.Repositories
{
    public class ItemCategoryRepository : IRepository<ItemCategory>, IManyToManyRelationship<ItemCategory>
    {
        private SQLiteAsyncConnection _database;

        public async Task Init()
        {
            if (_database != null)
                return;

            _database = new SQLiteAsyncConnection(DatabaseContext.DbPath, SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.Create | SQLiteOpenFlags.SharedCache);

            await _database.CreateTableAsync<ItemCategory>();
        }

        public async Task<ItemCategory> GetAsync(int id)
        {
            await Init();

            return await _database.Table<ItemCategory>().FirstOrDefaultAsync(i => i.Id == id);
        }

        public async Task<IEnumerable<ItemCategory>> GetAllAsync()
        {
            await Init();

            return await _database.Table<ItemCategory>().ToListAsync();
        }

        public async Task InsertAsync(ItemCategory entity)
        {
            await Init();

            await _database.InsertAsync(entity);
        }

        public async Task UpdateAsync(ItemCategory entity)
        {
            await Init();

            await _database.UpdateAsync(entity);
        }

        public async Task DeleteAsync(int id)
        {
            await Init();

            await _database.DeleteAsync<ItemCategory>(id);
        }

        public async Task<IEnumerable<ItemCategory>> GetByParent(int id)
        {
            await Init();

            return await _database.Table<ItemCategory>()
                .Where(i => i.ItemId == id)
                .ToListAsync(); ;
        }

        public async Task<IEnumerable<ItemCategory>> GetByChild(int id)
        {
            await Init();

            return await _database.Table<ItemCategory>()
                .Where(c => c.CategoryId == id)
                .ToListAsync();
        }
    }
}
