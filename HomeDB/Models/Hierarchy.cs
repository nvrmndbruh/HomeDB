using SQLite;

namespace HomeDB.Models
{
    public enum ChildType { Container, Item }

    public class Hierarchy
    {
        [AutoIncrement, PrimaryKey]
        public int Id { get; set; }
        public int ParentId { get; set; }
        public int ChildId { get; set; }
        public ChildType ChildType { get; set; }
    }
}
