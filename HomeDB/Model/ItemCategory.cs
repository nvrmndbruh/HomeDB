using SQLite;

namespace HomeDB.Model
{
    [Table("item_category")]
    public class ItemCategory
    {
        [PrimaryKey, AutoIncrement]
        [Column("id")]
        public int Id { get; set; }

        [Column("item_id")]
        public int ItemId { get; set; }

        [Column("category_id")]
        public int CategoryId { get; set; }
    }
}
