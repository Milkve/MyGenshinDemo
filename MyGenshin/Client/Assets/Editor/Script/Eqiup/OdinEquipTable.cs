using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Demos.RPGEditor;
using SkillBridge.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

namespace Editor
{
    public class OdinEquipTable : Singleton<OdinEquipTable>
    {

        const string GROUP = "Equip";


        [HorizontalGroup(GROUP,0.4f, PaddingRight =5)]
        [TableList(IsReadOnly = true, AlwaysExpanded = true), ShowInInspector]
        private List<OdinEquipWarpper> equips = null;

        private OdinEquip equip = new OdinEquip();


        [HorizontalGroup(GROUP, 0.6f)]
        [BoxGroup(GROUP + "/INFO", GroupName = "INFO")]
        [BoxGroup(GROUP + "/INFO/INFO", GroupName = "General Settings")]
        [VerticalGroup(GROUP + "/INFO/INFO/UP")]
        [HorizontalGroup(GROUP + "/INFO/INFO/UP/ICON", Width = 55)]
        [HideLabel, PreviewField(55, ObjectFieldAlignment.Left), ShowInInspector]
        public Texture Icon;

        [HorizontalGroup(GROUP + "/INFO/INFO/UP/ICON", Width = 100)]
        [VerticalGroup(GROUP + "/INFO/INFO/UP/ICON/UP")]
        [ShowInInspector, LabelWidth(30)]
        public int ID;

        [VerticalGroup(GROUP + "/INFO/INFO/UP/ICON/UP")]
        [ShowInInspector, LabelWidth(30)]
        public string Name;

        [VerticalGroup(GROUP + "/INFO/INFO/UP/ICON/UP")]
        [ShowInInspector, LabelWidth(30)]
        public EquipType Type;

        [BoxGroup(GROUP + "/INFO/Description", GroupName = "Description")]
        [HideLabel, TextArea]
        public string Description;


        public void Load(IEnumerable<OdinEquip> equips)
        {
            this.equips = equips.Select(x => new OdinEquipWarpper(x)).ToList();
            if (this.equips.Count != 0)
            {
                SetInfo(this.equips[0].Equip);
            }

        }

        public void SetInfo(OdinEquip equip)
        {
            this.Icon = equip.Icon;
            this.ID = equip.ID;
            this.Type = equip.Type;
            this.Name = equip.Name;
            this.Description=equip.Description;
        }

        private class OdinEquipWarpper
        {

            private OdinEquip equip;
            public OdinEquip Equip { get => equip; }
            public OdinEquipWarpper(OdinEquip equip)
            {
                this.equip = equip;
            }

            //[ShowInInspector, HideLabel, PreviewField(20, ObjectFieldAlignment.Left)]
            //public Texture Icon { get { return this.equip.Icon; } set { this.equip.Icon = value; EditorUtility.SetDirty(this.equip); } }
            //[LabelText("ID")]
            [ShowInInspector]
            public int ID => this.Equip.ID;

            //[LabelText("名称")]
            [ShowInInspector]
            public string Name => this.Equip.Name;

            //[LabelText("")]
            [Button("详情")]
            public void ShowDetial()
            {
                OdinEquipTable.Instance.SetInfo(this.Equip);
            }
        }
    }



}
