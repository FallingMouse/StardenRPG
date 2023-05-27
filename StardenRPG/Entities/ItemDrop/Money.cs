using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StardenRPG.Entities.ItemDrop
{
    public class Money
    {
        public int Amount { get; set; }

        public Money(int amount)
        {
            Amount = amount;
        }
    }
}
