using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common.Utils;
using SkillBridge.Message;
using static Common.Utils.SciValueUtil;

namespace Common.Data
{
    public struct PropertiesTemplate
    {
        public int ID;
        public int Min;
        public int Max;
        public int Split;
    }
    public class EquipDefine
    {

        public int ID;
        //装备类型
        public EquipType Type;
        public string Resource;
        public string Name;
        public string Description;
        public string Category { get; set; }
        public int Rare;
        public List<PropertiesTemplate> BasicProperties;  //基础词条
        public List<PropertiesTemplate> FixProperties;    //固定词条
        public List<PropertiesTemplate> RandomProperties; //随机词条
        public int SellPrice { get; set; }
        public List<int> SellCurrencyID { get; set; }
        public List<int> SellCurrencyPrice { get; set; }

    }
}
