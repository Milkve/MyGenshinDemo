using Common.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Common.Data
{
    public class PropertyDefine
    {
        public enum PropertyType
        {
            AddStatistic,
            AddSkill
        }

        public int ID;
        public string Name;
        public string Description;
        public int Precision;
        public PropertyType Type;
        public int SkillID;
        public List<int> SkillParams;

    }
}
