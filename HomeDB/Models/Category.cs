using SQLite;

namespace HomeDB.Models
{
    public class Category
    {
        [AutoIncrement, PrimaryKey]
        public int Id { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
    }
}