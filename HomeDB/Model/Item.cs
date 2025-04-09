using SQLite;

namespace HomeDB.Model
{
    [Table("item")]
    public class Item
    {
        [PrimaryKey, AutoIncrement]
        [Column("id")]
        public int Id { get; set; }

        [Column("name")]
        public string Name { get; set; }

        [Column("description")]
        public string Description { get; set; }

        [Column("icon")]
        public string Icon { get; set; }

        [Column("photo")]
        public string Photo { get; set; }

        [Column("price")]
        public double Price { get; set; }
    }
}
