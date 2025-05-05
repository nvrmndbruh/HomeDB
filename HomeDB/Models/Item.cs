using SQLite;

namespace HomeDB.Models
{
    public class Item
    {
        [AutoIncrement, PrimaryKey]
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Icon { get; set; }
        public string Photo { get; set; }
        public decimal Price { get; set; }
    }
}
