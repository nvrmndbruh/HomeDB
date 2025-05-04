using System.ComponentModel.DataAnnotations.Schema;

namespace HomeDB.Models
{
    public enum ChildType { Container, Item }

    [Table("hierarchy")]
    public class Hierarchy
    {
        [Column("id")]
        public int Id { get; set; }

        [Column("parent_id")]
        public int ParentId { get; set; }

        [Column("child_id")]
        public int ChildId { get; set; }

        [Column("child_type")]
        public ChildType ChildType { get; set; }

        public Container Parent { get; set; }
    }
}
