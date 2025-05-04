using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HomeDB.Models
{
    [Table("item")]
    public class Item
    {
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
        public decimal Price { get; set; }

        public List<ItemCategory> ItemCategories { get; set; } = new List<ItemCategory>();
    }
}
