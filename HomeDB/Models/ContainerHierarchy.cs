using SQLite;

namespace HomeDB.Models
{
    public class ContainerHierarchy
    {
        [AutoIncrement, PrimaryKey]
        public int Id { get; set; }
        [Indexed]
        public int ParentId { get; set; }
        [Indexed]
        public int ChildId { get; set; }
    }
}
