using Common.Data;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Managers
{

    public class NPCManager : Singleton<NPCManager>
    {

        public Action<NPCDefine> OnNpcEnter;

        public Action<NPCDefine> OnNpcLeave;

        public Dictionary<int, NPCController> AllNpcs = new Dictionary<int, NPCController>();

        public int CurrentTalkToID;

        public NPCController CurrentTalkToNpc { get { return AllNpcs[CurrentTalkToID]; } }

        private Queue<NPCController> initQueue = new Queue<NPCController>();
        Action<NPCController> LuaNpcInfo;
        
        public void  NpcInit()
        {
            QuestManager.Instance.OnQuestInitComplete += NpcInfoInit;
            QuestManager.Instance.OnNpcStatusChange += OnNpcStatusChange;
            LuaNpcInfo = LuaBehaviour.Instance.GetAction<Action<NPCController>>("AddChrInfo");
        }

        public void NpcInfoInit()
        {
            GameObject.FindObjectOfType<Coroutine_Runner>().StartCoroutine(NpcInfo());
        }


        public IEnumerator NpcInfo()
        {
            while (true)
            {
                while (initQueue.Count > 0)
                {
                    var npc = initQueue.Dequeue();
                    npc.Status.Value = QuestManager.Instance.GetNpcStatus(npc.npcID);
                    //Debug.LogWarning(npc.Status.Value);
                    LuaNpcInfo?.Invoke(npc);
                    yield return new WaitForEndOfFrame();
                }
                yield return new WaitForEndOfFrame();
            }
        }

        public void OnNpcStatusChange(int npcID)
        {
            NPCController npc;
            if(AllNpcs.TryGetValue(npcID,out npc))
            {
                npc.Status.Value = QuestManager.Instance.GetNpcStatus(npc.npcID);
            }
        }
        public NPCDefine AddNpc(int npcID, NPCController ctrl)
        {
            var define = DataManager.Instance.NPCs[npcID];
            AllNpcs[npcID] = ctrl;
            initQueue.Enqueue(ctrl);
            return define;
        }

        public void NpcEnter(int id)
        {
            OnNpcEnter?.Invoke(AllNpcs[id]?.Define);
        }

        public void NpcLeave(int id)
        {
            OnNpcLeave?.Invoke(AllNpcs[id]?.Define);
        }


        public void NpcActive()
        {
            AllNpcs[CurrentTalkToID]?.DoActive();
        }

        public void NpcInActive(int id)
        {
            AllNpcs[CurrentTalkToID]?.DoInActive();
        }
    }







}
