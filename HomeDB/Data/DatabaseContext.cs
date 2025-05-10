using HomeDB.Models;
using SQLite;

namespace HomeDB.Data
{
    public class DatabaseContext
    {
        SQLiteAsyncConnection _database;

        public static string DbPath { get; } =
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "HomeDB.db");


        async Task Init()
        {
            if (_database != null)
                return;

            _database = new SQLiteAsyncConnection(DbPath);

            await _database.CreateTableAsync<Item>();
            await _database.CreateTableAsync<Container>();
            await _database.CreateTableAsync<Hierarchy>();
            await _database.CreateTableAsync<Category>();
            await _database.CreateTableAsync<ItemCategory>();
        }

        #region Item methods
        public async Task<IEnumerable<Item>> GetItems()
        {
            await Init();

            var items = await _database.Table<Item>().ToListAsync();
            return items;
        }

        public async Task<Item> GetItem(int id)
        {
            await Init();

            var item = await _database.Table<Item>().FirstOrDefaultAsync(i => i.Id == id);
            return item;
        }

        public async Task UpdateItem(Item item)
        {
            await Init();
            await _database.UpdateAsync(item);
        }

        public async Task InsertItem(Item item)
        {
            await Init();
            await _database.InsertAsync(item);
        }

        public async Task DeleteItem(Item item)
        {
            await Init();

            // удаляем связанные записи из таблицы Hierarchy
            await _database.Table<Hierarchy>()
                .DeleteAsync(h => h.ChildId == item.Id && h.ChildType == nameof(Item));
            await _database.DeleteAsync(item);
        }
        #endregion

        #region Container methods
        public async Task<IEnumerable<Container>> GetContainers()
        {
            await Init();

            var containers = await _database.Table<Container>().ToListAsync();
            return containers;
        }

        public async Task<Container> GetContainer(int id)
        {
            await Init();

            var container = await _database.Table<Container>().FirstOrDefaultAsync(c => c.Id == id);
            return container;
        }

        public async Task UpdateContainer(Container container)
        {
            await Init();
            await _database.UpdateAsync(container);
        }

        public async Task InsertContainer(Container container)
        {
            await Init();
            await _database.InsertAsync(container);
        }

        public async Task DeleteContainer(Container container)
        {
            await Init();
            await _database.Table<Container>().DeleteAsync(c => c.Id == container.Id);
        }
        #endregion

        #region Hierarchy methods
        public async Task<IEnumerable<Hierarchy>> GetHierarchies()
        {
            await Init();

            var hierarchies = await _database.Table<Hierarchy>().ToListAsync();
            return hierarchies;
        }

        public async Task<Hierarchy> GetChildrenHierarchy(Container container)
        {
            await Init();

            var hierarchies = await _database.Table<Hierarchy>()
                .FirstOrDefaultAsync(c => c.ChildId == container.Id && c.ChildType == nameof(Container));
            return hierarchies;
        }

        public async Task<Hierarchy> GetChildrenHierarchy(Item item)
        {
            await Init();

            var hierarchies = await _database.Table<Hierarchy>()
                .FirstOrDefaultAsync(c => c.ChildId == item.Id && c.ChildType == nameof(Item));
            return hierarchies;
        }

        public async Task<IEnumerable<Hierarchy>> GetParentHierarchies(Container container)
        {
            await Init();

            var hierarchies = await _database.Table<Hierarchy>()
                .Where(p => p.ParentId == container.Id)
                .ToListAsync();
            return hierarchies;
        }

        public async Task UpdateHierarchy(int parentId, IEnumerable<Hierarchy> hierarchies)
        {
            foreach (var hierarchie in hierarchies)
            {
                hierarchie.ParentId = parentId;
                await _database.UpdateAsync(hierarchie);
            }
        }

        public async Task DeleteHierarchies(IEnumerable<Hierarchy> hierarchies)
        {
            foreach (var hierarchy in hierarchies)
                await _database.DeleteAsync(hierarchy);
        }
        #endregion
    }
}
