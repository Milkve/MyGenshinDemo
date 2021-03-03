using SkillBridge.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Common.Data
{
    public class ItemDefine


    {
        public enum ItemFunction{
            RecoverHP,
            RecoverMp,
            AddBuff,
            AddExp,
            AddMoney,
            AddItem,
            AddSkillPoint
        }



        public int ID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public ItemType Type { get; set; }
        public int Rare { get; set; }
        public string Category { get; set; }
        public bool CanUse { get; set; }
        public int CD { get; set; }
        public int Stacked { get; set; }
        public string Resource { get; set; }
        public int SellPrice { get; set; }
        public List<int> SellCurrencyID { get; set; }
        public List<int> SellCurrencyPrice { get; set; }
        public ItemFunction Function { get; set; }
        public int Param { get; set; }
        public List<int> Params { get; set; }
    }
}
