using Common.Data;
using GameServer.Core;
using GameServer.Managers;
using Interface;
using SkillBridge.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Entities
{
    class Character : CharacterBase,IPostProcess
    {
        public TCharacter Data;

        public ItemManager itemManager ;
        public StatusManager statusManager;
        public GoodsLimitManager goodsLimitManager;
        public EquipManager equipManager;
        public QuestManager questManager;
        public MessageManager messageManager;
        public FriendManager friendManager;
        public Character(CharacterType type,TCharacter cha):
            base(new Core.Vector3Int(cha.MapPosX, cha.MapPosY, cha.MapPosZ),new Core.Vector3Int(cha.MapDirection,0,0))
        {

            this.Data = cha;
            this.Info = new NCharacterInfo();
            this.Id = cha.ID;
            this.Info.Type = type;
            this.Info.Id = cha.ID;
            this.Info.EntityId = this.entityId;
            this.Info.ConfigId = cha.TID;
            this.Info.Name = cha.Name;
            this.Info.Level = cha.Level;     
            this.Info.Class = cha.Class;
            this.Info.mapId = cha.MapID;
            this.Info.Entity = this.EntityData;
            this.Info.Gold = this.Gold;
            this.Info.Equiped = cha.Equiped;
            this.Define = DataManager.Instance.Characters[cha.TID];
            this.itemManager = new ItemManager(this);
            this.itemManager.GetItemInfo(Info.Items);
            this.statusManager = new StatusManager(this);
            this.goodsLimitManager = new GoodsLimitManager(this);
            this.goodsLimitManager.GetGoodsLimits(Info.goodsLimits);
            this.equipManager = new EquipManager(this);
            this.equipManager.GetEquipInfos(Info.Equips);
            this.questManager = new QuestManager(this);
            this.questManager.GetQuestInfos(Info.Quests);
            this.messageManager = new MessageManager(this);
            this.messageManager.GetMessageInfos(Info.Messages);
            this.friendManager = new FriendManager(this);
            this.friendManager.GetFriendsInfo(Info.Friends);


        }

        public long Gold
        {
            get { return Data.Gold; }
            set { 
                if (this.Data.Gold == value) return;
                this.statusManager.AddGoldChange((int)(value-Data.Gold ));
                Data.Gold = value;
            }
        }

        public void PostProcess(NetMessage message)
        {
            this.statusManager.PostProcess(message);
            this.messageManager.PostProcess(message);
            this.friendManager.PostProcess(message);
        }


        public void Clear()
        {

            this.friendManager.UpdateInfo();

        }
    }
}
