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

            _database = new SQLiteAsyncConnection(DbPath, SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.Create | SQLiteOpenFlags.SharedCache);

            await _database.CreateTableAsync<Item>();
            await _database.CreateTableAsync<Container>();
            await _database.CreateTableAsync<Hierarchy>();
            await _database.CreateTableAsync<Category>();
            await _database.CreateTableAsync<ItemCategory>();
            await _database.ExecuteAsync("PRAGMA foreign_keys = ON;");
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

        public async Task DeleteItem(int id)
        {
            await Init();

            // удаляем связанные записи из таблицы Hierarchy
            await _database.Table<Hierarchy>()
                .DeleteAsync(h => h.ChildId == id && h.ChildType == nameof(Item));
            await _database.Table<Item>()
                .DeleteAsync(i => i.Id == id);
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

        public async Task DeleteContainer(int id)
        {
            await Init();
            await _database.Table<Container>().DeleteAsync(c => c.Id == id);
        }
        #endregion

        #region Hierarchy methods
        public async Task<IEnumerable<Hierarchy>> GetHierarchies()
        {
            await Init();

            var hierarchies = await _database.Table<Hierarchy>().ToListAsync();
            return hierarchies;
        }

        public async Task<Hierarchy> GetChildrenHierarchy(int childId, string childType)
        {
            await Init();

            var hierarchies = await _database.Table<Hierarchy>()
                .FirstOrDefaultAsync(c => c.ChildId == childId && c.ChildType == childType);
            return hierarchies;
        }

        public async Task<IEnumerable<Hierarchy>> GetParentHierarchies(int parentId)
        {
            await Init();

            var hierarchies = await _database.Table<Hierarchy>()
                .Where(p => p.ParentId == parentId)
                .ToListAsync();
            return hierarchies;
        }

        public async Task UpdateHierarchy(int parentId, IEnumerable<Hierarchy> hierarchies)
        {
            await Init();

            foreach (var hierarchie in hierarchies)
            {
                hierarchie.ParentId = parentId;
                await _database.UpdateAsync(hierarchie);
            }
        }

        public async Task DeleteHierarchies(IEnumerable<Hierarchy> hierarchies)
        {
            await Init();

            foreach (var hierarchy in hierarchies)
                await _database.DeleteAsync(hierarchy);
        }

        public async Task InsertHierarchy(Hierarchy hierarchy)
        {
            await Init();

            await _database.InsertAsync(hierarchy);
        }

        public async Task DeleteHierarchy(int id)
        {
            await Init();

            await _database.Table<Hierarchy>().DeleteAsync(h => h.Id == id);
        }
        #endregion

        #region Category methods
        public async Task<IEnumerable<Category>> GetCategories()
        {
            await Init();

            var categories = await _database.Table<Category>().ToListAsync();
            return categories;
        }

        public async Task<Category> GetCategory(int id)
        {
            await Init();

            var category = await _database.Table<Category>().FirstOrDefaultAsync(c => c.Id == id);
            return category;
        }

        public async Task UpdateCategory(Category category)
        {
            await Init();
            await _database.UpdateAsync(category);
        }

        public async Task InsertCategory(Category category)
        {
            await Init();
            await _database.InsertAsync(category);
        }

        public async Task DeleteCategory(int id)
        {
            await Init();
            await _database.Table<Category>().DeleteAsync(c => c.Id == id);
        }

        public async Task DeleteCategory(Category category)
        {
            await Init();
            await _database.DeleteAsync(category);
        }
        #endregion

        #region ItemCategory methods
        public async Task<IEnumerable<ItemCategory>> GetItemCategories()
        {
            await Init();

            var categories = await _database.Table<ItemCategory>().ToListAsync();
            return categories;
        }

        public async Task<ItemCategory> GetItemCategory(int id)
        {
            await Init();

            var category = await _database.Table<ItemCategory>().FirstOrDefaultAsync(c => c.Id == id);
            return category;
        }

        public async Task InsertItemCategory(ItemCategory category)
        {
            await Init();
            await _database.InsertAsync(category);
        }

        public async Task DeleteItemCategory(int id)
        {
            await Init();
            await _database.Table<ItemCategory>().DeleteAsync(c => c.Id == id);
        }
        #endregion
    }
}
