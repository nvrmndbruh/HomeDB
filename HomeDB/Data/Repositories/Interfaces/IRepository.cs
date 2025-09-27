namespace HomeDB.Data.Repositories.Interfaces
{
    public interface IRepository<T> where T : class
    {
        public Task Init();

        public Task<T> GetAsync(int id);

        public Task<IEnumerable<T>> GetAllAsync();

        public Task InsertAsync(T entity);

        public Task UpdateAsync(T entity);

        public Task DeleteAsync(int id);
    }
}