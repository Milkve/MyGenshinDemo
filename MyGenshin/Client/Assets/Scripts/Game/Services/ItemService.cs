using System;
using Network;
using SkillBridge.Message;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Managers;
using Interface;

namespace Services
{
    public class ItemService : Singleton<ItemService>, IDisposable
    {
        public EventWraperV2<Result,string> OnBuy =new EventWraperV2<Result, string>();
        public EventWraperV2<Result,string> OnSell=new EventWraperV2<Result, string>();
        public ItemService()
        {
            MessageDistributer.Instance.Subscribe<ItemBuyResponse>(OnBuyItem);
            MessageDistributer.Instance.Subscribe<ItemSellResponse>(OnSellItem);
                
        }
        public void Dispose()
        {
            MessageDistributer.Instance.Unsubscribe<ItemBuyResponse>(OnBuyItem);
            MessageDistributer.Instance.Unsubscribe<ItemSellResponse>(OnSellItem);
        }


        public void SendBuyItem(int goodsId, int count)
        {
            NetMessage netMessage = new NetMessage()
            {
                Request = new NetMessageRequest()
                {
                    itemBuyRequest = new ItemBuyRequest()
                    {
                        nGoodsInfo = new NGoodsInfo()
                        {
                            Id = goodsId,
                            Count = count
                        }
                    }
                }
            };
            NetClient.Instance.SendMessage(netMessage);
        }

        public void SendSellItem(List<ISelectable> selectables)
        {
            NetMessage netMessage = new NetMessage()
            {
                Request = new NetMessageRequest()
                {
                    itemSellRequest = new ItemSellRequest()
                }
            };

            foreach (var item in selectables)
            {
                if (item.isItem)
                {
                    netMessage.Request.itemSellRequest.nItemInfos.Add(new NItemInfo
                    {
                        Id = item.ID,
                        Count = item.SelectCount
                    }) ;
                }
                else
                {
                    netMessage.Request.itemSellRequest.nEquipInfos.Add(new NEquipInfo
                    {
                        Id = item.ID,
                    });
                }
            }

            NetClient.Instance.SendMessage(netMessage);
        }


        private void OnBuyItem(object sender, ItemBuyResponse message)
        {
            OnBuy?.Invoke(message.Result,message.Errormsg);
        }
        private void OnSellItem(object sender, ItemSellResponse message)
        {
            OnSell?.Invoke(message.Result, message.Errormsg);
        }
    }
}
