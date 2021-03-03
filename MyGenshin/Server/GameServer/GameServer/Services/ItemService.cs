using System;
using Common;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Network;
using SkillBridge.Message;
using GameServer.Managers;
using Common.Data;
using GameServer.Entities;

namespace GameServer.Services
{
    class ItemService : Singleton<ItemService>
    {

        public void Init()
        {
            MessageDistributer<NetConnection<NetSession>>.Instance.Subscribe<ItemBuyRequest>(this.OnItemBuy);
            MessageDistributer<NetConnection<NetSession>>.Instance.Subscribe<ItemSellRequest>(this.OnItemSell);
            MessageDistributer<NetConnection<NetSession>>.Instance.Subscribe<EquipWearRequest>(this.OnEquipUpdate);
        }

        private void OnEquipUpdate(NetConnection<NetSession> sender, EquipWearRequest message)
        {
            sender.Session.Response.equipResponse = new EquipWearResponse();

            if (message.Action == StatusAction.Add)
            {
                sender.Session.Response.equipResponse.Result=sender.Session.Character.equipManager.WearEquip(message.Id);
            }
            else if (message.Action == StatusAction.Delete) 
            {
                sender.Session.Response.equipResponse.Result=sender.Session.Character.equipManager.TakeOffEquip(message.Id);
            }
            sender.SendResponse();
        }

        private void OnItemSell(NetConnection<NetSession> sender, ItemSellRequest message)
        {
            sender.Session.Response.itemSellResponse = new ItemSellResponse();
            Result result = Result.Success;
            foreach (var item in message.nItemInfos)
            {
               result =result|StoreManager.Instance.SellItem(sender, item);
            }
            foreach (var equip in message.nEquipInfos)
            {
                result = result | StoreManager.Instance.SellEquip(sender, equip);
            }
            if (result == Result.Success) DBService.Instance.Save();
            sender.Session.Response.itemSellResponse.Result = result;
            sender.SendResponse();
        }

        private void OnItemBuy(NetConnection<NetSession> sender, ItemBuyRequest message)
        {
            sender.Session.Response.itemBuyResponse = new ItemBuyResponse();
            Result result = StoreManager.Instance.BuyItem(sender, message.nGoodsInfo);
            if (result == Result.Success) DBService.Instance.Save();
            sender.Session.Response.itemBuyResponse.Result = result;
            sender.SendResponse();
        }



    }
}
