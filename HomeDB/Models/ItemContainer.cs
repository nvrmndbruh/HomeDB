using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeDB.Models
{
    public class ItemContainer
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        [Indexed]
        public int ContainerId { get; set; }
        [Indexed]
        public int ItemId { get; set; }
    }
}
