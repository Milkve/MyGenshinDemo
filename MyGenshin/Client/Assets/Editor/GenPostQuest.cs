using Common.Data;
using Managers;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

namespace Assets
{
    public class GenPostQuest
    {

        [MenuItem("Tools/GenPostQuest")]
        public static void Gen()
        {

            DataManager.Instance.Load();
            var quests = DataManager.Instance.Quests;
            //先清空所有后置任务
            foreach (var key in quests.Keys)
            {
                quests[key].PostQuests = null;
            }

            foreach (var key in quests.Keys)
            {
                QuestDefine define = quests[key];
                if (define.PreQuests != null)
                {
                    for (int i = 0; i < define.PreQuests.Count; i++)
                    {
                        int preId = define.PreQuests[i];
                        if (quests.ContainsKey(preId))
                        {
                            if (quests[preId].PostQuests != null)
                            {
                                quests[preId].PostQuests.Add(define.ID);
                            }
                            else
                            {
                                quests[preId].PostQuests = new List<int>() { define.ID };
                            }
                        }
                    }
                }
            }

            string json = JsonConvert.SerializeObject(DataManager.Instance.Quests, Formatting.Indented);
            File.WriteAllText(SysDefine.PATH_DEFINE_QUEST, json);
            Debug.Log("后置任务生成完毕");

        }

    }
}
