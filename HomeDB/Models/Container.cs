using SQLite;

namespace HomeDB.Models
{
    public class Container
    {
        [AutoIncrement, PrimaryKey]
        public int Id { get; set; }
        public string Name { get; set; }
        public string Icon { get; set; }
    }
}
