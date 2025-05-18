using SQLite;

namespace HomeDB.Models
{
    [Table("Category")]
    public class Category
    {
        [AutoIncrement, PrimaryKey]
        [Column("category_id")]
        public int Id { get; set; }
        [Column("category_name")]
        public string Name { get; set; }
        [Column("category_description")]
        public string? Description { get; set; }
    }
}