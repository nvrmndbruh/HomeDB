using Microsoft.EntityFrameworkCore;

namespace HomeDB.Models
{
    public class HomeDbContext : DbContext
    {
        public DbSet<Container> Containers { get; set; }
        public DbSet<Item> Items { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Hierarchy> Hierarchies { get; set; }
        public DbSet<ItemCategory> ItemCategories { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=HomeDB.db");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Связь Hierarchy с Container (родитель)
            modelBuilder.Entity<Hierarchy>()
                .HasOne(h => h.Parent)
                .WithMany(c => c.ChildHierarchies)
                .HasForeignKey(h => h.ParentId);

            // Связь ItemCategory с Item и Category
            modelBuilder.Entity<ItemCategory>()
                .HasOne(ic => ic.Item)
                .WithMany(i => i.ItemCategories)
                .HasForeignKey(ic => ic.ItemId);

            modelBuilder.Entity<ItemCategory>()
                .HasOne(ic => ic.Category)
                .WithMany(c => c.ItemCategories)
                .HasForeignKey(ic => ic.CategoryId);
        }
    }
}
