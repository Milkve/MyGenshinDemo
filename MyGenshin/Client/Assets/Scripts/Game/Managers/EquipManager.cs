using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common.Utils;
using Entities;
using Models;
using Services;
using SkillBridge.Message;
using UnityEngine;

namespace Managers
{
    class EquipManager : Singleton<EquipManager>
    {
        public Dictionary<int, Equip> Equiped = new Dictionary<int, Equip>();
        public Dictionary<int, Equip> Equips = new Dictionary<int, Equip>();


        public EquipManager()
        {
            StatusService.Instance.RegisterEvent(StatusType.Equip, OnEquipStatusNotify);
            StatusService.Instance.RegisterEvent(StatusType.Equiped, OnEquipedStatusNotify);

        }
        public void Init(NCharacterInfo nCharacterInfo)
        {

            foreach (var item in nCharacterInfo.Equips)
            {
                Equips.Add(item.Id, new Equip(item));
            }


            for (int i = 0; i < nCharacterInfo.Equiped.Length / sizeof(int); i++)
            {
                Equip equip = null;
                if (Equips.TryGetValue(PointerUtil.ReadInt(nCharacterInfo.Equiped, i), out equip))
                {
                    WearEquip(equip, i);
                }
            }


        }

        private bool OnEquipedStatusNotify(NStatus nStatus)
        {
            Equip equip = null;
            if (!Equips.TryGetValue(nStatus.Value, out equip) || GameUtil.ParseSlot(equip.Type) != nStatus.Id) return false;       
            if (nStatus.Action == StatusAction.Add)
            {
                WearEquip(equip, nStatus.Id);
                return true;
            }
            if(nStatus.Action==StatusAction.Delete&& equip.CurrentSlot == nStatus.Id)
            {
                TakeOffEquip(equip, nStatus.Id);
            }
            return false;
        }

        private bool OnEquipStatusNotify(NStatus nStatus)
        {
            if (nStatus.Action == StatusAction.Add)
            {
                NEquipInfo nEquipInfo = new NEquipInfo()
                {
                    Id = nStatus.Id,
                    templateId = nStatus.Value,
                    Property = nStatus.Param
                };
                User.Instance.CurrentCharacter.Equips.Add(nEquipInfo);
                return AddEquip(nEquipInfo);
            }
            else if (nStatus.Action == StatusAction.Delete)
            {

                return RemoveEquip(nStatus.Id);
            }
            return false;
        }


        public bool AddEquip(NEquipInfo equipInfo)
        {
            if (Equips.ContainsKey(equipInfo.Id)) return false;
            Equip equip = new Equip(equipInfo);
            Equips.Add(equip.ID, equip);
            Debug.Log($"Add Equip {equip.ID}");
            return true;
        }

        public bool RemoveEquip(int EquipId)
        {
            return Equips.Remove(EquipId);
        }

        public bool WearEquip(Equip equip, int slot)
        {
            equip.CurrentSlot = slot;
            Equiped[slot] = equip;
            return true;
        }

        public bool TakeOffEquip(Equip equip,int slot)
        {
            equip.CurrentSlot = -1;
            Equiped.Remove(slot);
            return true;
        }

    }
}
