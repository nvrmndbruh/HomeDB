using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite;

namespace HomeDB.Model
{
    [Table("hierarchy")]
    public class Hierarchy
    {
        [PrimaryKey, AutoIncrement]
        [Column("id")]
        public int Id { get; set; }

        [Column("parent_id")]
        public int ParentId { get; set; }

        [Column("child_id")]
        public int ChildId { get; set; }

        [Column("child_type")]
        public string ChildType { get; set; }
    }
}
