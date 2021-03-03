using Common.Data;
using Common.Utils;
using Interface;
using Managers;
using SkillBridge.Message;
using System;
using System.Collections.Generic;

namespace Entities
{
    struct Property
    {
        public int ID;
        public int Value;
        public int Index;
        public PropertyDefine.PropertyType Type;
        public string Name;
        public string Descirption;
        public override string ToString()
        {
            string str = $@"ID:{ID}
                            Name:{Name}
                            Description:{Descirption}
                            Value:{Value}";

            return str;
        }



    }
    class Equip : ISelectable
    {
        public EquipDefine Define;
        public NEquipInfo Info;

        public int ID => Info.Id;
        public byte[] Property { get => Info.Property; private set => Info.Property = value; }
        public int Rare => Define.Rare;
        public int CurrentSlot = -1;
        public string Name => Define.Name;
        public EquipType Type => Define.Type;


        private bool singleSelect = false;
        private bool multipleSelect = false;
        public bool SingeSelect { get => singleSelect; set => singleSelect = value; }
        public bool MultipleSelect { get => multipleSelect; set => multipleSelect = value; }

        public bool isItem => false;
        private int selectCount = 1;
        public int SelectCount { get => selectCount; set => selectCount = 1; }

        public int Count => 1;

        public Equip(NEquipInfo nEquipInfo)
        {
            Info = nEquipInfo;
            if (!DataManager.Instance.Equips.TryGetValue(nEquipInfo.templateId, out Define))
            {

                throw new Exception("找不到对应Define");
            }

        }
        public List<Property> GetBasicProperties()
        {
            int start = 0;
            int end = Define.BasicProperties.Count;
            return Getproperty(start, end);
        }

        public List<Property> GetFixProperties()
        {
            int start = Define.BasicProperties.Count;
            int end = Define.BasicProperties.Count + Define.FixProperties.Count;
            return Getproperty(start, end);
        }
        public List<Property> GetRandomProperties()
        {
            int start = Define.BasicProperties.Count + Define.FixProperties.Count;
            int end = Define.BasicProperties.Count + Define.FixProperties.Count + GameUtil.GetPropertyCount(Rare);
            return Getproperty(start, end);
        }

        public List<Property> GetAllProperties()
        {
            int start = 0;
            int end = Define.BasicProperties.Count + Define.FixProperties.Count + GameUtil.GetPropertyCount(Rare);
            return Getproperty(start, end);
        }
        private List<Property> Getproperty(int start, int end)
        {
            List<Property> res = new List<Property>(end - start);
            for (int i = start; i < end; i++)
            {

                var kv = PointerUtil.ReadKShortVInt(this.Property, i);
                PropertyDefine propertyDefine;
                if (DataManager.Instance.Properties.TryGetValue(kv.Key, out propertyDefine))
                {
                    Property property = new Property()
                    {
                        ID = kv.Key,
                        Name = propertyDefine.Name,
                        Type = propertyDefine.Type,
                        Index = propertyDefine.Precision,
                        Descirption = propertyDefine.Description,
                        Value = kv.Value
                    };

                    res.Add(property);
                }

            }
            return res;
        }




    }
}
