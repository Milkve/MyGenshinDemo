using Common.Data;
using Managers;
using SkillBridge.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Utility;
using static Common.Data.QuestDefine;
using static Managers.QuestManager;

namespace Entities
{
    public class Quest
    {

        public int ID => Define.ID;

        public int Guid;
        public QuestType Type => Define.Type;

        public int Chapter => Define.Chapter;
        public BindableProperty<QuestStatus> Status = new BindableProperty<QuestStatus>(QuestStatus.None);

        public BindableProperty<NpcQuestStatus> NpcStatus;
        public QuestDefine Define;

        public List<BindableProperty<int>> ReachedTargets;
        public int LimitLevel => Define.LimitLevel;
        public List<int> LimitClass => Define.LimitClass;
        public string Description => Define.Description;
        public int SubmitNpc => Define.SubmitNpc;
        public int AcceptNpc => Define.AcceptNpc;
        public List<QuestTarget> Targets = new List<QuestTarget>() { };

        public List<int> PreQuests => Define.PreQuests;
        public List<int> PostQuests => Define.PostQuests;


        //根据服务端给的消息创建
        public Quest(NQuestInfo info, QuestDefine define) : this(define)
        {
            Guid = info.Guid;
            Status = new BindableProperty<QuestStatus>(info.Status);
            Status.OnValueChange.AddListener(OnStatusChange);
            ReachedTargets = info.Targets.Select(x => new BindableProperty<int>(x)).ToList();
            NpcStatus = new BindableProperty<NpcQuestStatus>(GetQuestNpcStatus(this));
            NpcStatus.OnValueChange.AddListener(OnNpcStatusChange);

        }

        private void OnStatusChange(QuestStatus old, QuestStatus @new)
        {
            Debug.Log($"OnQuestStatusChange: {@new}");
            QuestManager.Instance.CheckPostQueue(this);
            QuestManager.Instance.CheckQuestStatus();
        }

        private void OnNpcStatusChange(NpcQuestStatus old, NpcQuestStatus @new)
        {
            QuestManager.Instance.UpdateQuest(this, old, @new);
        }

        //根据define创建 仅用于显示 服务端没记录
        public Quest(QuestDefine define)
        {
            Define = define;
            NpcStatus = new BindableProperty<NpcQuestStatus>(NpcQuestStatus.None);
            Status.OnValueChange.AddListener(OnStatusChange);
            NpcStatus.OnValueChange.AddListener(OnNpcStatusChange);
            NpcStatus.Value = GetQuestNpcStatus(this);    
         
            ReachedTargets = new List<int>(){ 0,0,0}.Select(x => new BindableProperty<int>(x)).ToList();
        }
        //将用于显示的Quest 升级成正式的Quest
        public void Update(NQuestInfo info)
        {
            Guid = info.Guid;
            Status.Value = info.Status;
            ReachedTargets = info.Targets.Select(x => new BindableProperty<int>(x)).ToList();
            NpcStatus.Value = GetQuestNpcStatus(this);
        }
    }
}
