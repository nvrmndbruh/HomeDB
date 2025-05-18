using SQLite;

namespace HomeDB.Models
{
    [Table("Container")]
    public class Container
    {
        [AutoIncrement, PrimaryKey]
        [Column("container_id")]
        public int Id { get; set; }
        [Column("container_name")]
        public string Name { get; set; }
        [Column("container_icon")]
        public string Icon { get; set; }
    }
}
