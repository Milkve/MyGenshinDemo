using Common.Data;
using Entities;
using Models;
using Services;
using SkillBridge.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static Common.Data.QuestDefine;

namespace Managers
{
    public class QuestManager : Singleton<QuestManager>

    {
        // Quest状态：                          Quest       QuestStatus   NpcQuestStatus
        // 未接受 前置任务未完成              不实例化         Null           None
        // 未接受 前置任务已完成  等级不够      实例化         Null           Visible
        // 未接受 前置任务已完成  等级够        实例化         Null           Accpetable
        // 已接受 目标未完成                    实例化         InProgress     Incomplete
        // 已接受 目标已完成                    实例化         Complated      Complete
        // 已提交 任务结束                      实例化         Finished       None
        // 已失败                               实例化         Field          None
        public enum NpcQuestStatus   //优先度依次提高
        {
            None,           //不可见              前置任务未完成 
            Visible,        //可见不可接受        前置任务已完成 但等级未达到
            Incomplete,     //过程中
            Acceptable,     //可接受
            Complete,       //可完成
        }
        //任务初始化完毕事件
        public Action OnQuestInitComplete;

        public Dictionary<int, Quest> AllQuests = new Dictionary<int, Quest>();

        public Dictionary<int, List<Quest>> NpcStatus = new Dictionary<int, List<Quest>>();

        public Queue<int> QueueQuest = new Queue<int>();
        //NPC状态改变事件
        public Action<int> OnNpcStatusChange;
        /// <summary>
        /// 任务管理器初始化
        /// </summary>
        /// <param name="character"></param>
        public void QuestInit(NCharacterInfo character)
        {

            foreach (var item in character.Quests)
            {
                QuestDefine define;
                if (DataManager.Instance.Quests.TryGetValue(item.Id, out define))
                {
                    QueueQuest.Enqueue(item.Id);
                    AddQuest(item);
                }

            }
            foreach (var item in DataManager.Instance.Quests.Where(x => x.Value.PreQuests == null || x.Value.PreQuests.Count == 0))
            {
                QueueQuest.Enqueue(item.Value.ID);
            }
            CheckQuestStatus();
            OnQuestInitComplete?.Invoke();
        }
        /// <summary>
        /// 一个任务的NPC状态变化以后 需要对NPCStatus更新
        /// </summary>
        /// <param name="quest"></param>
        /// <param name="old"></param>
        /// <param name="new"></param>
        public void UpdateQuest(Quest quest, NpcQuestStatus old, NpcQuestStatus @new)
        {
            Debug.Log($"questID:{quest.ID} old:{old} new:{@new}");
            if (old == NpcQuestStatus.Visible || old == NpcQuestStatus.Acceptable || old == NpcQuestStatus.None)
            {
                RemoveQuestFromNpc(quest, quest.AcceptNpc);
                //NpcStatus[quest.AcceptNpc].Remove(quest);
            }
            else
            {
                RemoveQuestFromNpc(quest, quest.SubmitNpc);
                NpcStatus[quest.SubmitNpc].Remove(quest);
            }
            if (@new == NpcQuestStatus.Visible || @new == NpcQuestStatus.Acceptable || @new == NpcQuestStatus.None)
            {
                AddQuestToNpc(quest, quest.AcceptNpc);
            }
            else
            {
                AddQuestToNpc(quest, quest.SubmitNpc);
            }

            OnNpcStatusChange?.Invoke(quest.AcceptNpc);
            OnNpcStatusChange?.Invoke(quest.SubmitNpc);
        }
        /// <summary>
        /// 服务器过来的任务状态同步
        /// </summary>
        /// <param name="questInfo"></param>
        public void OnSyncQuest(NQuestInfo questInfo)
        {

            Quest quest;
            if (AllQuests.TryGetValue(questInfo.Id, out quest))
            {
                quest.Update(questInfo);
                return;
            }
            AddQuest(questInfo);


        }

        /// <summary>
        /// 用于显示
        /// 任务类别->任务所属章节->任务ID->任务
        /// </summary>
        /// <returns></returns>
        public Dictionary<QuestType, Dictionary<int, Dictionary<int, Quest>>> GetQuestByStatusAndChapter()
        {
            //我就想试试看能写的多恶心
            //发现这一步没有必要 但是已经写好了就懒得改了
            Dictionary<QuestType, Dictionary<int, Dictionary<int, Quest>>> res = new Dictionary<QuestType, Dictionary<int, Dictionary<int, Quest>>>();
            foreach (var typeGroup in from Quest in AllQuests
                                      where Quest.Value.Status.Value != QuestStatus.Finished
                                      where Quest.Value.Status.Value != QuestStatus.Failed
                                      group Quest by Quest.Value.Type
                                      into StatusGroups
                                      from ChapterGroup in
                                      from StatusGroup in StatusGroups
                                      group StatusGroup by StatusGroup.Value.Define.Chapter
                                      orderby StatusGroups.Key
                                      group ChapterGroup by StatusGroups.Key)
            {
                res[typeGroup.Key] = new Dictionary<int, Dictionary<int, Quest>>();
                foreach (var chapterGroup in typeGroup)
                {
                    res[typeGroup.Key][chapterGroup.Key] = new Dictionary<int, Quest>();
                    foreach (var quest in chapterGroup)
                    {
                        //Debug.Log($"ADD {typeGroup.Key} {chapterGroup.Key} {quest.Key} {quest.Value}");
                        res[typeGroup.Key][chapterGroup.Key].Add(quest.Key, quest.Value);
                    }
                }
            }
            return res;
        }

        public Dictionary<int, Quest> GetQuestForShow()
        {
            return AllQuests.OrderBy(x => x.Value.Type)
                .Where(x => x.Value.Status.Value != QuestStatus.Failed && x.Value.Status.Value != QuestStatus.Finished)
                .ToDictionary(x => x.Key, x => x.Value); ;


        }

        /// <summary>
        /// 根据任务的状态获取任务的NPC显示状态
        /// </summary>
        /// <param name="quest"></param>
        /// <returns></returns>
        public static NpcQuestStatus GetQuestNpcStatus(Quest quest)
        {
            if (quest.LimitClass.Contains(User.Instance.CurrentCharacter.Class))
            {
                return NpcQuestStatus.None;
            }
            //用于显示的Quest
            if (quest.Status.Value == QuestStatus.None)
            {
                if (User.Instance.CurrentCharacter.Level >= quest.LimitLevel)
                {
                    return NpcQuestStatus.Acceptable;
                }
                return NpcQuestStatus.Visible;
            }
            //任务完成或失败不显示
            if (quest.Status.Value == QuestStatus.Finished || quest.Status.Value == QuestStatus.Failed)
            {
                return NpcQuestStatus.None;
            }
            if (quest.Status.Value == QuestStatus.Complated)
            {
                return NpcQuestStatus.Complete;
            }
            return NpcQuestStatus.Incomplete;
        }

        /// <summary>
        /// 获取指定NPC目前的显示状态
        /// </summary>
        /// <param name="npcId"></param>
        /// <returns></returns>
        public NpcQuestStatus GetNpcStatus(int npcId)
        {
            List<Quest> list;

            if (NpcStatus.TryGetValue(npcId, out list))
            {
                NpcQuestStatus[] Statuses = (NpcQuestStatus[])Enum.GetValues(typeof(NpcQuestStatus));
                for (int i = Statuses.Length - 1; i >= 0; i--)
                {
                    if (list.Where(x => x.NpcStatus.Value == Statuses[i]).Count() > 0)
                    {
                        return Statuses[i];
                    }
                }
            }
            return NpcQuestStatus.None;
        }
        /// <summary>
        /// 指定npc应有的对话选项
        /// </summary>
        /// <param name="npcId"></param>
        /// <returns></returns>
        public Dictionary<int, int> GetTalkSelect(int npcId)
        {
            List<Quest> list;
            Dictionary<int, int> res = new Dictionary<int, int>();
            if (NpcStatus.TryGetValue(npcId, out list))
            {
                for (int i = 0; i < list.Count; i++)
                {
                    if (list[i].NpcStatus.Value == NpcQuestStatus.Acceptable)
                    {
                        //Debug.Log($"AddSelectSelect:{list[i].Define.AcceptTalk} {list[i].ID}");
                        res.Add(list[i].Define.AcceptTalk, list[i].ID);
                    }
                    else if (list[i].NpcStatus.Value == NpcQuestStatus.Complete)
                    {
                        //Debug.Log($"AddSelectSelect:{list[i].Define.AcceptTalk} {list[i].ID}");
                        res.Add(list[i].Define.SubmitTalk, list[i].ID);
                    }
                }

            }
            return res;
        }
        /// <summary>
        /// 实例化一个Quest后加入ALlQuest
        /// </summary>
        /// <param name="quest"></param>
        private void AddQuest(Quest quest)
        {
            if (!AllQuests.ContainsKey(quest.ID))
            {
                AllQuests.Add(quest.ID, quest);
                //int npcId = quest.SubmitNpc;
                //if (quest.NpcStatus.Value == NpcQuestStatus.Visible || quest.NpcStatus.Value == NpcQuestStatus.Acceptable || quest.NpcStatus.Value == NpcQuestStatus.None)
                //{
                //    npcId = quest.AcceptNpc;
                //}
                //AddQuest2NpcStatus(quest, npcId);
            }
        }
        private void AddQuest(NQuestInfo info)
        {
            QuestDefine define;
            if (DataManager.Instance.Quests.TryGetValue(info.Id, out define))
            {
                Quest quest = new Quest(info, define);
                AddQuest(quest);
            }
        }

        private void AddQuestToNpc(Quest quest, int npcId)
        {
            if (NpcStatus.ContainsKey(npcId))
            {
                NpcStatus[npcId].Add(quest);
            }
            else
            {
                NpcStatus[npcId] = new List<Quest>() { quest };
            }
        }
        private void RemoveQuestFromNpc(Quest quest, int npcId)
        {
            if (NpcStatus.ContainsKey(npcId))
            {
                NpcStatus[npcId].Remove(quest);
            }
        }


        #region 检查可接任务
        /// <summary>
        /// bfs搜索所有可接任务
        /// </summary>
        public void CheckQuestStatus()
        {
            //TODO:计算量如果大考虑搞成协程
            while (QueueQuest.Count > 0)
            {
                int id = QueueQuest.Dequeue();
                Quest quest;
                if (AllQuests.TryGetValue(id, out quest))
                {
                    CheckPostQueue(quest);
                    continue;
                }
                QuestDefine define;
                if (DataManager.Instance.Quests.TryGetValue(id, out define))
                {
                    quest = new Quest(define);
                    AddQuest(quest);
                }
            }

        }
        public void CheckPostQueue(Quest quest)
        {   //如果任务已经完成 就遍历后置任务加入检查队列
            if (quest.Status.Value == QuestStatus.Finished && quest.PostQuests != null)
            {
                quest.PostQuests.ForEach(x => AddQueue(x));
            }
        }
        private void AddQueue(int id)
        {
            //后置任务不存在 或者 不在检查队列中
            if (!AllQuests.ContainsKey(id) && !QueueQuest.Contains(id))
            {
                QueueQuest.Enqueue(id);
            }
        }
        #endregion
        #region 发送请求
        /// <summary>
        /// 发送任务接受请求
        /// </summary>
        /// <param name="id"></param>
        public void SendAccept(int id)
        {
            Debug.Log($"SendQuestAccept: QuestID{ id}");
            Quest quest;
            if (AllQuests.TryGetValue(id, out quest))
            {
                if (quest.NpcStatus.Value == NpcQuestStatus.Acceptable && quest.Status.Value == QuestStatus.None)
                {
                    QuestService.Instance.SendAccept(id);
                }

            }
        }/// <summary>
         /// 发送任务提交请求
         /// </summary>
         /// <param name="id"></param>
        public void SendSubmit(int id)
        {
            Quest quest;
            if (AllQuests.TryGetValue(id, out quest))
            {
                if (quest.NpcStatus.Value == NpcQuestStatus.Complete && quest.Status.Value == QuestStatus.Complated)
                {
                    QuestService.Instance.SendSubmit(id);
                }

            }
        }
        #endregion
    }
}
