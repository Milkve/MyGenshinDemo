using SkillBridge.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Common.Utils
{
    public static  class GameUtil
    {


        /// <summary>
        /// 根据稀有度判断随机词条的数量
        /// </summary>
        /// <param name="rare">稀有度</param>
        /// <returns>词条数量</returns>
        public static int GetPropertyCount(int rare)
        {
            return rare;
        }

        /// <summary>
        /// 根据装备类型获取对应的槽位ID
        /// </summary>
        /// <param name="type">装备类型</param>
        /// <returns>槽位ID</returns>
        public static int ParseSlot(EquipType type)
        {
            if (((int)type & 2) != 0) return 0;
            return ((int)type >> 2) + 1;
        }
    }
}
