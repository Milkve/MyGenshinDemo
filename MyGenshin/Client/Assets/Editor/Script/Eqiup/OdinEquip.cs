using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Demos.RPGEditor;
using SkillBridge.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Editor
{
    public class OdinEquip
    {
        public struct Property
        {

        }
        public Texture Icon;
        public int ID;
        public string Name;
        public string Description;
        public EquipType Type;
        public List<Property> Properties;



    }
}
