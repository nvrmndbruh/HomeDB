using SQLite;

namespace HomeDB.Models
{
    [Table("Item")]
    public class Item
    {
        [AutoIncrement, PrimaryKey]
        [Column("item_id")]
        public int Id { get; set; }
        [Column("item_name")]
        public string Name { get; set; }
        [Column("item_description")]
        public string? Description { get; set; }
        [Column("item_icon")]
        public string Icon { get; set; }
        [Column("photo")]
        public string? Photo { get; set; }
        [Column("price")]
        public decimal? Price { get; set; }
    }
}


