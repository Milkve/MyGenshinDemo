using GameServer.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SkillBridge.Message;
using Common.Data;
using static Common.Data.GoodsDefine;
using GameServer.Services;

namespace GameServer.Managers
{
    class GoodsLimitManager
    {

        public Character Owner;

        public Dictionary<int, GoodsLimit> GoodsLimits = new Dictionary<int, GoodsLimit>();


        public GoodsLimitManager(Character character)
        {
            Owner = character;
            foreach (var goodslimit in character.Data.GoodsLimits)
            {
                GoodsLimits.Add(goodslimit.GoodsID, new GoodsLimit(goodslimit));
            }
        }

        public void GetGoodsLimits(List<NGoodsLimit> nGoodsLimits)
        {
            foreach (var goodLimit in GoodsLimits)
            {
                nGoodsLimits.Add(new NGoodsLimit()
                {
                    goodsId = goodLimit.Value.GoodsID,
                    Count = goodLimit.Value.Count

                });
                //Console.WriteLine($"NGoodsLimit:{nGoodsLimits.Last().Count}");
            }
        }
        public bool CanBuy(NGoodsInfo goodsInfo, GoodsDefine goodsDefine)
        {
            if (goodsDefine.LimitType == GoodsLimitType.None) return true;
            GoodsLimit goodsLimit = null;
            if (GoodsLimits.TryGetValue(goodsInfo.Id, out goodsLimit))
            {
                if (goodsLimit.Count + goodsInfo.Count <= goodsDefine.Limit)
                {
                    return true;
                }
            }
            if (goodsInfo.Count <= goodsDefine.Limit)
            {
                return true;
            }
            return false;
        }
        public int GetLimit(int id)
        {
            GoodsLimit goodsLimit = null;
            if (GoodsLimits.TryGetValue(id, out goodsLimit))
            {
                return goodsLimit.Count;
            }
            return 0;
        }
        public void Apply(NGoodsInfo goodsInfo, GoodsDefine goodsDefine)
        {
            GoodsLimit goodsLimit = null;
            if (GoodsLimits.TryGetValue(goodsInfo.Id, out goodsLimit))
            {
                if (goodsLimit.Count + goodsInfo.Count <= goodsDefine.Limit)
                {
                    goodsLimit.Add(goodsInfo.Count);
                }
            }
            else if (goodsInfo.Count <= goodsDefine.Limit)
            {
                TGoodsLimit limit = new TGoodsLimit()
                {
                    GoodsID = goodsInfo.Id,
                    Purchased = (short)goodsInfo.Count,
                    TCharacter = Owner.Data
                };
                Owner.Data.GoodsLimits.Add(limit);
                GoodsLimits.Add(goodsInfo.Id, new GoodsLimit(limit));
            }

            Owner.statusManager.AddStatus(StatusType.Goodslimit, StatusAction.Add, goodsInfo.Id, goodsInfo.Count);

        }

    }
}
