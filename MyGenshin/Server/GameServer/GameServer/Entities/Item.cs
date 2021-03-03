using Common.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Entities
{
    class Item
    {
        public int Id;
        public TCharacterItem tcharacterItem;
        public int Count;
        public Item (TCharacterItem Titem)
        {
            this.tcharacterItem = Titem;
            this.Id = Titem.ItemID;
            this.Count = Titem.Count;
        }

        public void Add(int count)
        {
            Count += count;
            tcharacterItem.Count =Count;
        }

        public void Remove(int count)
        {
            Count -= count;
            tcharacterItem.Count = Count;
        }
    }
}
