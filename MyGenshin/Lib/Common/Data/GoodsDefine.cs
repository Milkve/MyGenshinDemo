using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Common.Data
{
    public class GoodsDefine
    {
        public enum GoodsLimitType
        {
            None,  //无限制
            Zone, //单次副本
            Day,    //每天
            Week,   //每周
            Month,  //每月
            Always, //永久
        }
        public enum GoodsType { 
            Item,
            Equip
        }

        public int ID { get; set; }

        public int StoreID { get; set; }
        public int ItemID { get; set; }

        public int Count { get; set; }
        public int Price { get; set; }

        public List<int> CurrencyID { get; set; }
        public List<int> CurrencyPrice { get; set; }

        public bool isActive { get; set; }

        public GoodsLimitType LimitType { get; set; } 

        public GoodsType Type { get; set; }
        public int Limit { get; set; }

    }
}
