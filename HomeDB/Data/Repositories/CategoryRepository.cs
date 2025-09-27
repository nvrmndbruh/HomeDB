using HomeDB.Data.Repositories.Interfaces;
using HomeDB.Models;
using SQLite;

namespace HomeDB.Data.Repositories
{
    public class CategoryRepository : IRepository<Category>
    {
        private SQLiteAsyncConnection _database;

        public async Task Init()
        {
            if (_database != null)
                return;

            _database = new SQLiteAsyncConnection(DatabaseContext.DbPath, SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.Create | SQLiteOpenFlags.SharedCache);

            await _database.CreateTableAsync<Category>();
        }

        public async Task<Category> GetAsync(int id)
        {
            await Init();

            return await _database.Table<Category>().FirstOrDefaultAsync(i => i.Id == id);
        }
        public async Task<IEnumerable<Category>> GetAllAsync()
        {
            await Init();

            return await _database.Table<Category>().ToListAsync();
        }

        public async Task InsertAsync(Category entity)
        {
            await Init();

            await _database.InsertAsync(entity);
        }

        public async Task UpdateAsync(Category entity)
        {
            await Init();

            await _database.UpdateAsync(entity);
        }

        public async Task DeleteAsync(int id)
        {
            await Init();

            await _database.DeleteAsync<Category>(id);
        }
    }
}
