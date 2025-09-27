using HomeDB.Data.Repositories.Interfaces;
using HomeDB.Models;
using SQLite;

namespace HomeDB.Data.Repositories
{
    public class HierarchyRepository : IRepository<Hierarchy>, IOneToManyRelationship<Hierarchy>
    {
        private SQLiteAsyncConnection _database;

        public async Task Init()
        {
            if (_database != null)
                return;

            _database = new SQLiteAsyncConnection(DatabaseContext.DbPath, SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.Create | SQLiteOpenFlags.SharedCache);

            await _database.CreateTableAsync<Hierarchy>();
        }

        public async Task<Hierarchy> GetAsync(int id)
        {
            await Init();

            return await _database.Table<Hierarchy>().FirstOrDefaultAsync(i => i.Id == id);
        }

        public async Task<IEnumerable<Hierarchy>> GetAllAsync()
        {
            await Init();

            return await _database.Table<Hierarchy>().ToListAsync();
        }

        public async Task InsertAsync(Hierarchy entity)
        {
            await Init();

            await _database.InsertAsync(entity);
        }

        public async Task UpdateAsync(Hierarchy entity)
        {
            await Init();

            await _database.UpdateAsync(entity);
        }

        public async Task DeleteAsync(int id)
        {
            await Init();

            await _database.DeleteAsync<Hierarchy>(id);
        }

        public async Task<IEnumerable<Hierarchy>> GetByParent(int id)
        {
            await Init();

            return await _database.Table<Hierarchy>()
                .Where(p => p.ParentId == id)
                .ToListAsync(); ;
        }

        public async Task<Hierarchy> GetByChild(int id)
        {
            await Init();

            return await _database.Table<Hierarchy>()
                .FirstOrDefaultAsync(c => c.ChildId == id); ;
        }
    }
}
