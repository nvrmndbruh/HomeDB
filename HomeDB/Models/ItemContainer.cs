using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeDB.Models
{
    [Table("Item_Container")]
    public class ItemContainer
    {
        [PrimaryKey, AutoIncrement]
        [Column("item_container_id")]
        public int Id { get; set; }
        [Indexed]
        [Column("container_id")]
        public int ContainerId { get; set; }
        [Indexed]
        [Column("item_id")]
        public int ItemId { get; set; }
    }
}
