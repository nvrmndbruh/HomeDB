namespace HomeDB.Data.Repositories.Interfaces
{
    public interface IOneToManyRelationship<T> where T : class
    {
        public Task<IEnumerable<T>> GetByParent(int id);

        public Task<T> GetByChild(int id);
    }
}
