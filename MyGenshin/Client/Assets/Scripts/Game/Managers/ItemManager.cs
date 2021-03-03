using Common.Data;
using Entities;
using Models;
using Services;
using SkillBridge.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using static Services.StatusService;

namespace Managers
{
    class ItemManager : Singleton<ItemManager>
    {
        public Dictionary<int, Item> Items = new Dictionary<int, Item>();

        public Dictionary<int, GoodsLimit> Limits = new Dictionary<int, GoodsLimit>();

        long gold;
        public Action<long> OnGoldChange;
        public long Gold { get => gold; set { gold = value; OnGoldChange?.Invoke(value); } }

        //public long Gold { get {ret ; private set { } }
        public ItemManager()
        {
            StatusService.Instance.RegisterEvent(StatusType.Item, OnItemStatusNotify);
            StatusService.Instance.RegisterEvent(StatusType.Gold, OnGoldStatusNotify);
            StatusService.Instance.RegisterEvent(StatusType.Goodslimit, OnLimitStatusNotify);
        }

        private bool OnLimitStatusNotify(NStatus nStatus)
        {
            if (nStatus.Action == StatusAction.Add)
            {
                this.AddLimit(nStatus.Id, nStatus.Value);
            }
            return true;
        }

        private bool OnGoldStatusNotify(NStatus nStatus)
        {
            if (nStatus.Action == StatusAction.Add)
            {
                Gold += nStatus.Value;
            }
            else if (nStatus.Action == StatusAction.Delete)
            {
                Gold -= nStatus.Value;
            }
            return true;
        }

        private bool OnItemStatusNotify(NStatus nStatus)
        {
            if (nStatus.Action == StatusAction.Add)
            {
                this.AddItem(nStatus.Id, nStatus.Value);
            }
            else if (nStatus.Action == StatusAction.Delete)
            {
                this.RemoveItem(nStatus.Id, nStatus.Value);
            }
            return true;
        }

        public void Init(NCharacterInfo nCharacter)
        {
            Gold = nCharacter.Gold;
            List<NItemInfo> Nitems = nCharacter.Items;
            this.Items.Clear();
            foreach (var item in Nitems)
            {
                this.Items.Add(item.Id, new Item(item));
            }
            this.Limits.Clear();
            foreach (var item in nCharacter.goodsLimits)
            {

                this.Limits.Add(item.goodsId, new GoodsLimit(item));
            }
        }
        public bool HasItem(int itemId, int count = 1)
        {
            Item item = null;
            if (Items.TryGetValue(itemId, out item))
            {
                return item.Count >= count;
            }
            return false;
        }
        public Item GetItem(int itemId)
        {
            Item item = null;
            Items.TryGetValue(itemId, out item);
            return item;
        }



        public bool UseItem(int itemId, int count = 1)
        {
            Item item = null;
            if (Items.TryGetValue(itemId, out item))
            {
                if (item.Count < count) return false;
                //use
                item.Remove(count);
                return true;

            }
            return false;
        }

        void AddItem(int itemId, int count = 1)
        {
            Item item = null;
            if (Items.TryGetValue(itemId, out item))
            {
                item.Add(count);
            }
            else
            {
                NItemInfo nItemInfo = new NItemInfo()
                {
                    Id = itemId,
                    Count = count
                };
                item = new Item(nItemInfo);
                User.Instance.CurrentCharacter.Items.Add(nItemInfo);
                Items.Add(itemId, item);
            }

        }
        void RemoveItem(int itemId, int count = 1)
        {
            Item item = null;
            if (Items.TryGetValue(itemId, out item))
            {
                if (item.Count < count) return;
                item.Remove(count);
                if (item.Count == 0)
                {
                    Items.Remove(itemId);
                }
            }
        }

        void AddLimit(int goodsId, int count = 1)
        {
            GoodsLimit limit = null;
            if (Limits.TryGetValue(goodsId, out limit))
            {
                limit.Add(count);
            }
            else
            {
                limit = new GoodsLimit(goodsId, count);
                Limits.Add(goodsId, limit);
            }

        }

        public GoodsLimit GetGoodsLimit(int goodsId)
        {
            GoodsLimit limit = null;
            if (Limits.TryGetValue(goodsId, out limit))
            {
                return limit;
            }
            limit = new GoodsLimit(goodsId, 0);
            Limits.Add(goodsId, limit);
            return limit;

        }

    }
}
