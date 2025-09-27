using HomeDB.Data.Repositories.Interfaces;
using HomeDB.Models;
using SQLite;

namespace HomeDB.Data.Repositories
{
    public class ContainerRepository : IRepository<Container>
    {
        private SQLiteAsyncConnection _database;

        public async Task Init()
        {
            if (_database != null)
                return;

            _database = new SQLiteAsyncConnection(DatabaseContext.DbPath, SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.Create | SQLiteOpenFlags.SharedCache);

            await _database.CreateTableAsync<Container>();
        }

        public async Task<Container> GetAsync(int id)
        {
            await Init();

            return await _database.Table<Container>().FirstOrDefaultAsync(i => i.Id == id);
        }
        public async Task<IEnumerable<Container>> GetAllAsync()
        {
            await Init();

            return await _database.Table<Container>().ToListAsync();
        }

        public async Task InsertAsync(Container entity)
        {
            await Init();

            await _database.InsertAsync(entity);
        }

        public async Task UpdateAsync(Container entity)
        {
            await Init();

            await _database.UpdateAsync(entity);
        }

        public async Task DeleteAsync(int id)
        {
            await Init();

            await _database.DeleteAsync<Container>(id);
        }
    }
}
