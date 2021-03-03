using Common.Data;
using GameServer.Core;
using GameServer.Managers;
using SkillBridge.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Entities
{
    class Character : CharacterBase
    {
       
        public TCharacter Data;

        public ItemManager itemManager ;
        public StatusManager statusManager;
        public GoodsLimitManager goodsLimitManager;
        public EquipManager equipManager;
        public QuestManager questManager;
        public Character(CharacterType type,TCharacter cha):
            base(new Core.Vector3Int(cha.MapPosX, cha.MapPosY, cha.MapPosZ),new Core.Vector3Int(cha.MapDirection,0,0))
        {

            this.Data = cha;
            this.Info = new NCharacterInfo();
            this.Info.Type = type;
            this.Info.Id = cha.ID;
            this.Info.Name = cha.Name;
            this.Info.Level = cha.Level;
            this.Info.Tid = cha.TID;
            this.Info.Class = cha.Class;
            this.Info.mapId = cha.MapID;
            this.Info.Entity = this.EntityData;
            this.Info.Gold = this.Gold;
            this.Info.Equiped = cha.Equiped;
            this.itemManager = new ItemManager(this);
            this.itemManager.GetItemInfo(Info.Items);
            this.statusManager = new StatusManager(this);
            this.goodsLimitManager = new GoodsLimitManager(this);
            this.goodsLimitManager.GetGoodsLimits(Info.goodsLimits);
            this.equipManager = new EquipManager(this);
            this.equipManager.GetEquipInfo(Info.Equips);
            this.questManager = new QuestManager(this);
            this.questManager.GetQuestInfos(Info.Quests);

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

    }
}
