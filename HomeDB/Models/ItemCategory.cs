using SQLite;

namespace HomeDB.Models
{
    public class ItemCategory
    {
        [AutoIncrement, PrimaryKey]
        public int Id { get; set; }
        public int ItemId { get; set; }
        public int CategoryId { get; set; }
    }
}
