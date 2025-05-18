using SQLite;

namespace HomeDB.Models
{
    [Table("Item_Category")]
    public class ItemCategory
    {
        [AutoIncrement, PrimaryKey]
        [Column("item_category_id")]
        public int Id { get; set; }
        [Indexed]
        [Column("item_id")]
        public int ItemId { get; set; }
        [Indexed]
        [Column("category_id")]
        public int CategoryId { get; set; }
    }
}
