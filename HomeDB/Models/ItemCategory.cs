using SQLite;

namespace HomeDB.Models
{
    public class ItemCategory
    {
        [AutoIncrement, PrimaryKey]
        public int Id { get; set; }

        [Indexed]
        public int ItemId { get; set; }

        [Indexed]
        public int CategoryId { get; set; }
    }
}
