using SQLite;

namespace HomeDB.Models
{
    [Table("Hierarchy")]
    public class Hierarchy
    {
        [AutoIncrement, PrimaryKey]
        [Column("hierarchy_id")]
        public int Id { get; set; }
        [Indexed]
        [Column("parent_id")]
        public int ParentId { get; set; }
        [Indexed]
        [Column("child_id")]
        public int ChildId { get; set; }
    }
}
