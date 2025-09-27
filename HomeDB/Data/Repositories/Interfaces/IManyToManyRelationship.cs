namespace HomeDB.Data.Repositories.Interfaces
{
    public interface IManyToManyRelationship<T> where T : class
    {
        public Task<IEnumerable<T>> GetByParent(int id);

        public Task<IEnumerable<T>> GetByChild(int id);
    }
}
