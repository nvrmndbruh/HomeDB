using SQLite;

namespace HomeDB.Models
{
    [Table("category")]
    public class Category
    {
        [PrimaryKey, AutoIncrement]
        [Column("id")]
        public int Id { get; set; }

        [Column("name")]
        public string Name { get; set; }

        //[Column("icon")]
        //public string Icon { get; set; }

        [Column("description")]
        public string Description { get; set; }
    }
}