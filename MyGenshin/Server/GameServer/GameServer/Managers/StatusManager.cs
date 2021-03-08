using GameServer.Entities;
using Interface;
using SkillBridge.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Managers
{
    class StatusManager:IPostProcess
    {
        Character Owner;
        List<NStatus> Statuses;
        int gold = 0;
        Dictionary<int, int> items = new Dictionary<int, int>();
        public bool Dirty = false;

        public StatusManager(Character character)
        {

            Owner = character;
            Statuses = new List<NStatus>();
        }

        public NStatus AddStatus(StatusType type, StatusAction action, int id, int value, byte[] param = null)
        {
            Dirty = true;
            NStatus nstatus = new NStatus()
            {
                Type = type,
                Action = action,
                Id = id,
                Value = value,
                Param = param
            };
            Console.WriteLine($"{type} {action} {id} {value}");
            Statuses.Add(nstatus);
            return nstatus;
        }

        public void AddGoldChange(int count)

        {
            Dirty = true;
            gold += count;
        }

        public void AddEquipedChange(int slot, int equipID, StatusAction action)
        {
            Dirty = true;
            AddStatus(StatusType.Equiped, action, slot, equipID);

        }
        public void AddEquipChange(StatusAction action, int id, int templateId, byte[] property = null)
        {
            Dirty = true;
            AddStatus(StatusType.Equip, action, id, templateId, property);
        }

        public void AddItemChange(int id, int count)
        {
            Dirty = true;
            if (items.ContainsKey(id))
            {
                items[id] += count;
            }
            else
            {
                items[id] = count;
            }
            //if (count > 0)
            //{
            //    AddStatus(StatusType.Item, StatusAction.Add, id, count);
            //}
            //else
            //{
            //    AddStatus(StatusType.Item, StatusAction.Delete, id, -count);
            //}
        }
        public void ApplyResponse(NetMessageResponse response)
        {
            if (response.statusNotify == null)
            {
                response.statusNotify = new StatusNotify();
            }
            if (gold != 0)
            {
                if (gold > 0)
                {
                    AddStatus(StatusType.Gold, StatusAction.Add, 0, gold);

                }
                else
                {
                    AddStatus(StatusType.Gold, StatusAction.Delete, 0, -gold);
                }
                gold = 0;
            }

            if (items.Count > 0)
            {
                foreach (var item in items)
                {
                    if (item.Value > 0)
                    {
                        AddStatus(StatusType.Item, StatusAction.Add, item.Key, item.Value);
                    }
                    else if (item.Value < 0)
                    {
                        AddStatus(StatusType.Item, StatusAction.Delete, item.Key, -item.Value);
                    }

                }
                items.Clear();
            }

            foreach (var item in Statuses)
            {
                response.statusNotify.Status.Add(item);
            }
            Statuses.Clear();
            Dirty = false;
        }

        public void PostProcess(NetMessage message)
        {
            if (this.Dirty)
            {
              ApplyResponse(message.Response);
            }
        }
    }
}
