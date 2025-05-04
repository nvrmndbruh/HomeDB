using System.ComponentModel.DataAnnotations.Schema;

namespace HomeDB.Models
{
    [Table("item_category")]
    public class ItemCategory
    {
        [Column("id")]
        public int Id { get; set; }

        [Column("item_id")]
        public int ItemId { get; set; }

        [Column("category_id")]
        public int CategoryId { get; set; }

        public Item Item { get; set; }
        public Category Category { get; set; }
    }
}
