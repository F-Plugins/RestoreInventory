using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestoreInventory.Models
{
    [Serializable]
    public class PlayerItem
    {
        public ushort Id { get; set; }
        public byte Durability { get; set; }
        public string? Metadata { get; set; }
        public byte Amount { get; set; }
        public byte Quality { get; set; }
        public string? State { get; set; }
        public byte Page { get; set; }
        public byte X { get; set; }
        public byte Y { get; set; }
        public byte Rot { get; set; }
    }
}
