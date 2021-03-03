using Common.Data;
using Interface;
using Managers;
using SkillBridge.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Entities
{
    class Item:ISelectable
    {
        public int ID => Info.Id;
        public int Count { get => Info.Count; set => Info.Count = value; }
        public NItemInfo Info;
        public ItemDefine Define => DataManager.Instance.Items[Info.Id];
        public int Rare => Define.Rare;

        private bool singleSelect=false;
        private bool multipleSelect=false;
        public bool SingeSelect { get => singleSelect; set => singleSelect=value; }
        public bool MultipleSelect { get => multipleSelect; set => multipleSelect=value; }
        public bool isItem  => true;
        private int selectCount=1;
        public int SelectCount { get =>selectCount; set =>selectCount=value; }

        public EventWraperV2<int,int> OnChange= new EventWraperV2<int, int>();
        public Item(NItemInfo nItemInfo)
        {
            Info = nItemInfo;
        }

        public void Add(int count)
        {
            OnChange?.Invoke(this.Count,this.Count+count);
            this.Count += count;
            
        }

        public void Remove(int count)
        {
            OnChange?.Invoke(this.Count,this.Count-count);
            this.Count -= count;
            
        }
    }
}
