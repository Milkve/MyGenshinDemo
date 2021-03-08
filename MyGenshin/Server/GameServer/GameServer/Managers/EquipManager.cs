using Common.Data;
using Common.Utils;
using GameServer.Entities;
using GameServer.Services;
using SkillBridge.Message;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Managers
{
    class EquipManager
    {
        private int EquipID=0;
        Dictionary<int, Equip> Equiped = new Dictionary<int, Equip>();
        Dictionary<int, Equip> Equips = new Dictionary<int, Equip>();
        Character Owner;
        public EquipManager(Character owner)
        {
            Owner = owner;
            foreach (var item in Owner.Data.CharacterEquips)
            {
                Equip equip = new Equip(item);
                equip.Id = ++EquipID;
                Equips.Add(equip.Id,equip );
            }
            for (int i = 0; i < 10; i++)
            {
                int equipID = PointerUtil.ReadInt(owner.Info.Equiped, i);
                Equip equip = null;
                if(Equips.TryGetValue(equipID, out equip))
                {
                    equip.CurrentSlot = i;
                    Equiped[i] = equip;
                }     

            }

        }


        public void GetEquipInfos(List<NEquipInfo> equips)
        {
            foreach (var item in this.Equips)
            {
                NEquipInfo info = new NEquipInfo()
                {
                    Id = item.Value.Id,
                    Property = item.Value.Property,
                    templateId = item.Value.TemplateId
                };
                equips.Add(info);
            }
        }


        public static NEquipInfo GetEquipInfo(int equipId)
        {
            TCharacterEquip tequip = DBService.Instance.Entities.CharacterEquip.Where(x => x.Id == equipId).FirstOrDefault();
            if (tequip != null)
            {
                return GetEquipInfo(tequip);
            }
            return default;
        }
        public static NEquipInfo GetEquipInfo(TCharacterEquip tequip)
        {
            return new NEquipInfo()
            {
                Id = tequip.Id,
                Property = tequip.Property,
                templateId = tequip.TemplateID
            };
        }
        public static NEquipInfo GetEquipInfo(Equip equip)
        {
            return new NEquipInfo()
            {
                Id = equip.Id,
                Property = equip.Property,
                templateId = equip.Define.ID
            };
        }

        public bool AddEquip(int templateID, int count)
        {
            EquipDefine define = null;

            if (DataManager.Instance.Equips.TryGetValue(templateID, out define))
            {
                bool flag = true;
                for (int i = 0; i <count; i++)
                {
                    flag=flag &this.AddEquip(EquipInstance(define));
                }
                return flag;
            }
            return false;
        }

        public bool AddEquip(Equip equip)
        {
            equip.Id = ++EquipID;
            Console.WriteLine(equip.Id);
            Owner.Data.CharacterEquips.Add(equip.TEquip);
            Equips.Add(equip.Id, equip);
            Owner.statusManager.AddEquipChange(StatusAction.Add, equip.Id, equip.Define.ID, equip.Property);
            return true;
        }

        public bool RemoveEquip(int equipId)
        {
            Equip equip = null;
            if (Equips.TryGetValue(equipId, out equip))
            {
                return RemoveEquip(equip);
            }
            return false;
        }
        public bool RemoveEquip(Equip equip)
        {
            Equips.Remove(equip.Id);
            if (equip.TEquip.Character == Owner.Data)
            {
                equip.TEquip.IsDelete = true;
            }
            return true;
        }


        public Result WearEquip(int equipid)
        {
            Equip equip=null;
            if(Equips.TryGetValue(equipid,out equip))
            {
               return WearEquip(equip);
            }
            return Result.Failed;       
        }

        public bool HasEquip(int id)
        {
            return Equips.ContainsKey(id);
        }

        public Result WearEquip(Equip equip)
        {
            int slot = GameUtil.ParseSlot(equip.Define.Type);
            Equip cur;
            if(Equiped.TryGetValue(slot,out cur))
            {
                if (TakeOffEquip(cur) == Result.Failed)
                {
                    return Result.Failed;
                }
            }
            Equiped[slot] = equip;
            PointerUtil.WriteInt(Owner.Data.Equiped, slot, equip.Id);
            Owner.statusManager.AddEquipedChange(slot, equip.Id, StatusAction.Add);
            return Result.Success;
        }

        public Result TakeOffEquip(int equipid)
        {
            Equip equip = null;
            if (Equips.TryGetValue(equipid, out equip))
            {
                return TakeOffEquip(equip);
            }
            return Result.Failed;
        }

        public Result TakeOffEquip(Equip equip)
        {
            int slot = GameUtil.ParseSlot(equip.Define.Type);
            if (equip.CurrentSlot!=slot || Equiped[slot] != equip)
            {
                return Result.Failed;
            }
            Equiped.Remove(slot);
            PointerUtil.WriteInt(Owner.Data.Equiped, slot, 0);
            Owner.statusManager.AddEquipedChange(slot, equip.Id, StatusAction.Delete);
            return Result.Success;
        }





        public Equip EquipInstance(EquipDefine define)
        {
            TCharacterEquip TEquip = new TCharacterEquip()
            {
                TemplateID = define.ID,
                Property = new byte[10 * (sizeof(short) + sizeof(int))]
            };
            int i = 0;
            foreach (var item in define.BasicProperties)
            {
                PropertyRandom(TEquip.Property, i, item);
                i += 1;
            }
            foreach (var item in define.FixProperties)
            {
                PropertyRandom(TEquip.Property, i, item);
                i += 1;
            }
            int[] select = this.SelectProperty(define.RandomProperties.Select(x => x.ID).ToList(),GameUtil.GetPropertyCount(define.Rare));
            foreach (var item in define.RandomProperties.Where(x => select.Contains(x.ID)))
            {
                PropertyRandom(TEquip.Property, i, item);
                i += 1;
            }
            return new Equip(TEquip);

        }

        public bool PropertyRandom(byte[] data, int i, PropertiesTemplate property)
        {
            PropertyDefine define;
            if (DataManager.Instance.Properties.TryGetValue(property.ID, out define))
            {
                int n = RandomUtil.Gen.Next(0, property.Split + 1);
                int final = property.Min + (int)((property.Max - property.Min) * ((float)n / property.Split));
                PointerUtil.WriteKShortVInt(data, i, (short)property.ID, final);
                return true;
            }
            return false;
        }

        public int[] SelectProperty(List<int> total, int select)
        {
            int[] copy = total.ToArray();
            int[] res = new int[select];
            for (int i = 0; i < select; i++)
            {
                int n = RandomUtil.Gen.Next(i+1, copy.Length);
                copy[i] ^= copy[n];
                copy[n] ^= copy[i];
                copy[i] ^= copy[n];
                res[i] = copy[i];
            }
            return res;
        }




    }
}
