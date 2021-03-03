using Common.Data;
using GameServer.Managers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Entities
{
    public class Equip
    {

        public TCharacterEquip TEquip;
        public int Id ;
        public int TId => TEquip.Id;
        public int TemplateId    => TEquip.TemplateID;
        public EquipDefine Define=> DataManager.Instance.Equips[TemplateId];
        public byte[] Property   => TEquip.Property;
        public Equip(TCharacterEquip tEquip)
        {
            TEquip = tEquip;
        }
        public int CurrentSlot = -1;


    }
}
