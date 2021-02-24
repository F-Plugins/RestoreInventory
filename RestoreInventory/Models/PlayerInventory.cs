using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestoreInventory.Models
{
    public class PlayerInventory
    {
        public string? PlayerId { get; set; }
        public List<PlayerItem>? Items { get; set; }
        public List<ushort>? Clothing { get; set; }
        public DateTime Date { get; set; }
    }
}
