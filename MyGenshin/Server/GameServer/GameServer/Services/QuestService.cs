using Common;
using Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SkillBridge.Message;
namespace GameServer.Services
{
    class QuestService:Singleton<QuestService>,IDisposable
    {

        public void Init()
        {

        }
        public QuestService()
        {

            MessageDistributer<NetConnection<NetSession>>.Instance.Subscribe<QuestAcceptRequest>(this.OnQuestAccept);
            MessageDistributer<NetConnection<NetSession>>.Instance.Subscribe<QuestListRequest>(this.OnQuestList);
            MessageDistributer<NetConnection<NetSession>>.Instance.Subscribe<QuestSubmitRequest>(this.OnQuestSubmit);

        }
        public void Dispose()
        {
            MessageDistributer<NetConnection<NetSession>>.Instance.Unsubscribe<QuestAcceptRequest>(this.OnQuestAccept);
            MessageDistributer<NetConnection<NetSession>>.Instance.Unsubscribe<QuestListRequest>(this.OnQuestList);
            MessageDistributer<NetConnection<NetSession>>.Instance.Unsubscribe<QuestSubmitRequest>(this.OnQuestSubmit);
        }


        private void OnQuestAccept(NetConnection<NetSession> sender, QuestAcceptRequest message)
        {
            QuestAcceptResponse response = new QuestAcceptResponse();
            sender.Session.Response.questAcceptResponse = response;
            response.Result = sender.Session.Character.questManager.OnAcceptQuest(sender, message.QuestId);
            DBService.Instance.Save();
            sender.SendResponse();
        }
        private void OnQuestSubmit(NetConnection<NetSession> sender, QuestSubmitRequest message)
        {
            QuestSubmitResponse response = new QuestSubmitResponse();
            sender.Session.Response.questSubmitResponse = response;
            response.Result = sender.Session.Character.questManager.OnSubmitQuest(sender, message.QuestId);
            DBService.Instance.Save();
            sender.SendResponse();
        }

        private void OnQuestList(NetConnection<NetSession> sender, QuestListRequest message)
        {

        }




    }
}
