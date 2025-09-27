using HomeDB.Data.Repositories;

namespace HomeDB.Data
{
    public class DatabaseContext
    {
        private const string databaseName = "HomeDB.db";

        public static string DbPath { get; } =
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), databaseName);

        public static ItemRepository Items => new ItemRepository();

        public static ContainerRepository Containers => new ContainerRepository();

        public static CategoryRepository Categories => new CategoryRepository();

        public static HierarchyRepository Hierarchies => new HierarchyRepository();

        public static ItemContainerRepository ItemContainers => new ItemContainerRepository();

        public static ItemCategoryRepository ItemCategories => new ItemCategoryRepository();
    }
}
