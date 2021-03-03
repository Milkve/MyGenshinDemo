using Sirenix.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Editor
{

    [GlobalConfig("Editor/Model/Equip", UseAsset = true)]
    public class OdinEquipOverview : GlobalConfig<OdinEquipOverview>
    {

        public OdinEquip[] Equips;


        public void UpdateOverview()
        {
            Equips = new OdinEquip[3] { new OdinEquip() { ID = 1, Name = "1" }, new OdinEquip() { ID = 2, Name = "2" }, new OdinEquip() { ID = 3, Name = "3" } };
        }

    }
}
