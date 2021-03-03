using GameServer.Managers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Common.Data.GoodsDefine;

namespace GameServer.Entities
{
    class GoodsLimit

    {
        public TGoodsLimit tGoodsLimit;
        public int GoodsID;
        public int Count { get; private set; }
        public GoodsLimitType LimitType;
        public GoodsLimit(TGoodsLimit tGLimit)
        {
            tGoodsLimit = tGLimit;
            GoodsID = tGLimit.GoodsID;
            Count = tGLimit.Purchased;
            //Console.WriteLine($"GoodsLimit:{Count}");
            LimitType = DataManager.Instance.Goods[GoodsID].LimitType;
        }

        public void Add(int count)
        {
            Count += count;
            tGoodsLimit.Purchased += (short)count;
        }
        public void ReSet()
        {
            Count = 0;
            tGoodsLimit.Purchased = 0;
        }
    }
}
