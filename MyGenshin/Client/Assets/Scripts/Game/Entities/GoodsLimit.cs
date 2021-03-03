using Common.Data;
using Managers;
using SkillBridge.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Common.Data.GoodsDefine;

namespace Entities
{
    class GoodsLimit
    {
        public int id;
        public int count { get; private set; }
        public int limit => this.define.Limit;

        public int canBuy => this.limit - count;
        public GoodsLimitType type => this.define.LimitType;
        public GoodsDefine define;
        public Action<int> OnChange;
        public GoodsLimit(NGoodsLimit nGoodsLimit) : this(nGoodsLimit.goodsId, nGoodsLimit.Count)
        {
        }
        public GoodsLimit(int goodsId, int count)
        {
            id = goodsId;
            this.count = count;
            define = DataManager.Instance.Goods[goodsId];
        }

        public void Add(int count)
        {
            this.count += count;
            OnChange?.Invoke(this.count);
        }
    }
}
