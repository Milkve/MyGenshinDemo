using Common.Data;
using GameServer.Entities;
using GameServer.Services;
using Network;
using SkillBridge.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Common.Data.QuestDefine;

namespace GameServer.Managers
{
    class QuestManager
    {
        Character Owner;

        public QuestManager(Character character)
        {
            Owner = character;

        }

        public void GetQuestInfos(List<NQuestInfo> quests)
        {
            foreach (var quest in Owner.Data.Quests)
            {
                quests.Add(GetQuestInfo(quest));
            }
        }

        public NQuestInfo GetQuestInfo(TQuest tquest)
        {

            NQuestInfo questInfo = new NQuestInfo()
            {
                Guid = tquest.Id,
                Id = tquest.QuestID,
                Status = (QuestStatus)tquest.Status,
                Targets = new int[] { tquest.Target1, tquest.Target2, tquest.Target2 }
            };
            return questInfo;
        }

        public Result OnAcceptQuest(NetConnection<NetSession> sender,int questID)
        {
            QuestDefine define;
            if (DataManager.Instance.Quests.TryGetValue(questID, out define))
            {
                TQuest dbquest = new TQuest();
                dbquest.QuestID = define.ID;
                if (define.Targets.FirstOrDefault().Type == QuestTargetType.None)
                {
                    dbquest.Status = (int)QuestStatus.Complated;
                }
                else
                {
                    dbquest.Status = (int)QuestStatus.InProgress;
                }
                sender.Session.Response.questAcceptResponse.Quest = GetQuestInfo(dbquest);
                Owner.Data.Quests.Add(dbquest);
                return Result.Success;
            }
            sender.Session.Response.questAcceptResponse.Errormsg = "任务不存在";
            return Result.Failed;
        }

        internal Result OnSubmitQuest(NetConnection<NetSession> sender, int questID)
        {
            QuestDefine define;
            if (DataManager.Instance.Quests.TryGetValue(questID, out define))
            {
                var dbquest = Owner.Data.Quests.Where(x => x.QuestID == questID).FirstOrDefault();
                if (dbquest != null)
                {
                    if (dbquest.Status != (int)QuestStatus.Complated)
                    {
                        sender.Session.Response.questSubmitResponse.Errormsg = "任务未完成";
                        return Result.Failed;
                    }
                    dbquest.Status = (int)QuestStatus.Finished;
                    sender.Session.Response.questSubmitResponse.Quest = GetQuestInfo(dbquest);
                    foreach ( var reward in define.Rewards)
                    {
                        switch (reward.Type)
                        {
                            case RewardType.Equip:
                                Owner.equipManager.AddEquip(reward.ID, reward.Value);break;
                            case RewardType.Exp:
                                //TODO:
                                break;
                            case RewardType.Gold:
                                Owner.Gold += reward.Value;break;
                            case RewardType.Item:
                                Owner.itemManager.AddItem(reward.ID, reward.Value);break;
                            default:break;
                        }
                    }

                    return Result.Success;
                }
            }
            sender.Session.Response.questSubmitResponse.Errormsg = "任务不存在";
            return Result.Failed;
        }
    }
}
