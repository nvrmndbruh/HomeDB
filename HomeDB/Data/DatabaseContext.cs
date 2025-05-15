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
            await _database.CreateTableAsync<ContainerHierarchy>();
            await _database.CreateTableAsync<Category>();
            await _database.CreateTableAsync<ItemCategory>();
            await _database.CreateTableAsync<ItemContainer>();
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

        public async Task DeleteItem(Item item)
        {
            await Init();

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
            await _database.DeleteAsync(container);
        }
        #endregion

        #region Hierarchy methods
        public async Task<IEnumerable<ContainerHierarchy>> GetHierarchies()
        {
            await Init();

            var hierarchies = await _database.Table<ContainerHierarchy>().ToListAsync();
            return hierarchies;
        }

        public async Task<ContainerHierarchy> GetChildrenHierarchy(int childId)
        {
            await Init();

            var hierarchies = await _database.Table<ContainerHierarchy>()
                .FirstOrDefaultAsync(c => c.ChildId == childId);
            return hierarchies;
        }

        public async Task<IEnumerable<ContainerHierarchy>> GetParentHierarchies(int parentId)
        {
            await Init();

            var hierarchies = await _database.Table<ContainerHierarchy>()
                .Where(p => p.ParentId == parentId)
                .ToListAsync();
            return hierarchies;
        }

        public async Task UpdateHierarchies(int parentId, IEnumerable<ContainerHierarchy> hierarchies)
        {
            await Init();

            foreach (var hierarchy in hierarchies)
            {
                hierarchy.ParentId = parentId;
                await _database.UpdateAsync(hierarchy);
            }
        }

        public async Task DeleteHierarchies(IEnumerable<ContainerHierarchy> hierarchies)
        {
            await Init();

            foreach (var hierarchy in hierarchies)
                await _database.DeleteAsync(hierarchy);
        }

        public async Task InsertHierarchy(ContainerHierarchy hierarchy)
        {
            await Init();

            await _database.InsertAsync(hierarchy);
        }

        public async Task DeleteHierarchy(int id)
        {
            await Init();

            await _database.Table<ContainerHierarchy>().DeleteAsync(h => h.Id == id);
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

        #region ItemContainer methods
        public async Task<IEnumerable<ItemContainer>> GetItemContainers()
        {
            await Init();

            var categories = await _database.Table<ItemContainer>().ToListAsync();
            return categories;
        }

        public async Task<ItemContainer> GetItemContainerByItem(int id)
        {
            await Init();

            var category = await _database.Table<ItemContainer>().FirstOrDefaultAsync(ic => ic.ItemId == id);
            return category;
        }

        public async Task<IEnumerable<ItemContainer>> GetItemContainerByContainer(int id)
        {
            await Init();

            var itemCategories = await _database.Table<ItemContainer>()
                .Where(ic => ic.ContainerId == id)
                .ToListAsync();
            return itemCategories;
        }

        public async Task InsertItemContainer(ItemContainer itemContainer)
        {
            await Init();
            await _database.InsertAsync(itemContainer);
        }

        public async Task UpdateItemContainers(int containerId, IEnumerable<ItemContainer> itemContainers)
        {
            await Init();
            foreach (var itemContainer in itemContainers)
            {
                itemContainer.ContainerId = containerId;
                await _database.UpdateAsync(itemContainer);
            }
        }

        public async Task DeleteItemContainer(int id)
        {
            await Init();
            await _database.Table<ItemContainer>().DeleteAsync(c => c.Id == id);
        }
        #endregion
    }
}
