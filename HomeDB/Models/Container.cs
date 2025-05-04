using System.ComponentModel.DataAnnotations.Schema;

namespace HomeDB.Models
{
    [Table("container")]
    public class Container
    {
        [Column("id")]
        public int Id { get; set; }

        [Column("name")]
        public string Name { get; set; }

        [Column("icon")]
        public string Icon { get; set; }

        public List<Hierarchy> ChildHierarchies { get; set; } = new List<Hierarchy>();
    }
}
