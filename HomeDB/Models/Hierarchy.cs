using SQLite;

namespace HomeDB.Models
{
    public class Hierarchy
    {
        [AutoIncrement, PrimaryKey]
        public int Id { get; set; }
        [Indexed]
        public int ParentId { get; set; }
        [Indexed]
        public int ChildId { get; set; }
        public string ChildType { get; set; }
    }
}
