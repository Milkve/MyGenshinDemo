using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SkillBridge.Message;
using Network;
using Managers;
using UnityEngine;

namespace Services
{
    class QuestService : Singleton<QuestService>, IDisposable
    {

        public QuestService()
        {
            MessageDistributer.Instance.Subscribe<QuestAcceptResponse>(OnQuestAccept);
            MessageDistributer.Instance.Subscribe<QuestSubmitResponse>(OnQuestSubmit);
            MessageDistributer.Instance.Subscribe<QuestAbandonResponse>(OnQuestAbandon);
        }

        public void Dispose()
        {
            MessageDistributer.Instance.Unsubscribe<QuestAcceptResponse>(OnQuestAccept);
            MessageDistributer.Instance.Unsubscribe<QuestSubmitResponse>(OnQuestSubmit);
            MessageDistributer.Instance.Unsubscribe<QuestAbandonResponse>(OnQuestAbandon);
        }

        public void SendAccept(int id)
        {
            Debug.Log($"ServiceSendQuestAccept: QuestID{ id}");
            NetMessage message = new NetMessage()
            {
                Request = new NetMessageRequest()
                {
                    questAcceptRequest = new QuestAcceptRequest()
                    {
                        QuestId = id
                    }
                }
            };
            NetClient.Instance.SendMessage(message);
        }
        public void SendSubmit(int id)
        {
            NetMessage message = new NetMessage()
            {
                Request = new NetMessageRequest()
                {
                    questSubmitRequest = new QuestSubmitRequest()
                    {
                        QuestId = id
                    }
                }
            };
            NetClient.Instance.SendMessage(message);
        }

        private void OnQuestAccept(object sender, QuestAcceptResponse message)
        {
            if (message.Result == Result.Success)
            {
                QuestManager.Instance.OnSyncQuest(message.Quest);
            }

        }



        private void OnQuestAbandon(object sender, QuestAbandonResponse message)
        {
        }

        private void OnQuestSubmit(object sender, QuestSubmitResponse message)
        {
            if (message.Result == Result.Success)
            {
                QuestManager.Instance.OnSyncQuest(message.Quest);
            }
        }
    }
}
