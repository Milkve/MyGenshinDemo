using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace Common.Data
{
    public class QuestDefine
    {
        public enum QuestType
        {
            [Description("主线任务")]
            Main,
            [Description("支线任务")]
            Branch,
            [Description("日常任务")]
            Repeated,
            [Description("世界任务")]
            World

        }

        public enum QuestTargetType
        {
            None,
            Kill,
            Item,
        }
        public enum RewardType
        {
            Gold,
            Exp,
            Item,
            Equip
        }
        public struct QuestTarget
        {
            public QuestTargetType Type;    //目标类型
            public string Description;      //描述
            public int ID;                  //目标ID
            public int Value;

        }

        public struct Reward
        {
            public RewardType Type;
            public int ID;
            public int Value;
        }

        public int ID { get; set; }
        public string Name { get; set; }

        public QuestType Type { get; set; }
        public string Description { get; set; }

        public int LimitLevel { get; set; }
        public List<int> LimitClass { get; set; }
        public int Chapter { get; set; }

        public List< QuestTarget> Targets { get; set; }

        public int AcceptTalk { get; set; }
        public int SubmitTalk { get; set; }

        public List<int> PreQuests { get; set; }

        public List<int> PostQuests { get; set; }//自动生成

        public int AcceptNpc { get; set; }

        public int SubmitNpc { get; set; }

        public List<Reward> Rewards { get; set; }


    }
}
