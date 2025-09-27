using HomeDB.Data.Repositories.Interfaces;
using HomeDB.Models;
using SQLite;

namespace HomeDB.Data.Repositories
{
    public class ItemContainerRepository : IRepository<ItemContainer>, IOneToManyRelationship<ItemContainer>
    {
        private SQLiteAsyncConnection _database;

        public async Task Init()
        {
            if (_database != null)
                return;

            _database = new SQLiteAsyncConnection(DatabaseContext.DbPath, SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.Create | SQLiteOpenFlags.SharedCache);

            await _database.CreateTableAsync<ItemContainer>();
        }

        public async Task<ItemContainer> GetAsync(int id)
        {
            await Init();

            return await _database.Table<ItemContainer>().FirstOrDefaultAsync(i => i.Id == id);
        }

        public async Task<IEnumerable<ItemContainer>> GetAllAsync()
        {
            await Init();

            return await _database.Table<ItemContainer>().ToListAsync();
        }

        public async Task InsertAsync(ItemContainer entity)
        {
            await Init();

            await _database.InsertAsync(entity);
        }

        public async Task UpdateAsync(ItemContainer entity)
        {
            await Init();

            await _database.UpdateAsync(entity);
        }

        public async Task DeleteAsync(int id)
        {
            await Init();

            await _database.DeleteAsync<ItemContainer>(id);
        }

        public async Task<IEnumerable<ItemContainer>> GetByParent(int id)
        {
            await Init();

            return await _database.Table<ItemContainer>()
                .Where(p => p.ContainerId == id)
                .ToListAsync(); ;
        }

        public async Task<ItemContainer> GetByChild(int id)
        {
            await Init();

            return await _database.Table<ItemContainer>()
                .FirstOrDefaultAsync(c => c.ItemId == id); ;
        }
    }
}
