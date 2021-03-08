using Common;
using Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SkillBridge.Message;
using GameServer.Managers;

namespace GameServer.Services
{
    class MessageService : Singleton<MessageService>, IDisposable
    {
        public void Init()
        {

        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public MessageService()
        {
            MessageDistributer<NetConnection<NetSession>>.Instance.Subscribe<MessageSendRequest>(this.OnMessageSend);
            MessageDistributer<NetConnection<NetSession>>.Instance.Subscribe<MessageAcceptRequest>(this.OnMessageAccept);
            MessageDistributer<NetConnection<NetSession>>.Instance.Subscribe<MessageDeleteRequest>(this.OnMessageDelete);
            MessageDistributer<NetConnection<NetSession>>.Instance.Subscribe<MessageListRequest>(this.OnMessageList);
            MessageDistributer<NetConnection<NetSession>>.Instance.Subscribe<MessageTargetInfoRequest>(this.OnMessageTargetInfo);
            MessageDistributer<NetConnection<NetSession>>.Instance.Subscribe<FriendListRequest>(this.OnFriendList);
            MessageDistributer<NetConnection<NetSession>>.Instance.Subscribe<FriendRemoveRequest>(this.OnFriendRemove);

        }

        private void OnMessageDelete(NetConnection<NetSession> sender, MessageDeleteRequest message)
        {
            throw new NotImplementedException();
        }

        private void OnMessageList(NetConnection<NetSession> sender, MessageListRequest message)
        {
            throw new NotImplementedException();
        }

        private void OnMessageTargetInfo(NetConnection<NetSession> sender, MessageTargetInfoRequest message)
        {
            var info = MessageManager.GetMessageCharInfo(message.Id);
            sender.Session.Response.messageTargetInfoResponse = new MessageTargetInfoResponse()
            {
                Result =info!=null?Result.Success:Result.Failed,
                Info = info,
                Errormsg = info != null ? string.Empty :"找不到玩家",
            };
            sender.SendResponse();
        }

        private void OnFriendList(NetConnection<NetSession> sender, FriendListRequest message)
        {
            throw new NotImplementedException();
        }

        private void OnFriendRemove(NetConnection<NetSession> sender, FriendRemoveRequest message)
        {
            throw new NotImplementedException();
        }

        private void OnMessageAccept(NetConnection<NetSession> sender, MessageAcceptRequest message)
        {
            sender.Session.Response.messageAcceptResponse = new MessageAcceptResponse();
            Result res = sender.Session.Character.messageManager.OnMessageAccept(sender, message);
            if (res == Result.Success) DBService.Instance.Save();
            sender.Session.Response.messageAcceptResponse.Result = res;
        }

        private void OnMessageSend(NetConnection<NetSession> sender, MessageSendRequest message)
        {
            sender.Session.Response.messageSendResponse = new MessageSendResponse();
            Result res = sender.Session.Character.messageManager.OnMessageSend(sender,message);
            if (res==Result.Success) DBService.Instance.Save();
            sender.Session.Response.messageSendResponse.Result = res;
            sender.SendResponse();
        }



        public void SendAddFriendResponse(NetConnection<NetSession> sender ,Result result,NMessageCharInfo info)
        {
            string msg = result ==Result.Success ? "接受" : "拒绝";
            sender.Session.Response.friendAdd = new FriendAddResponse()
            {
                Result = result,
                Errormsg = $"玩家[{info.Name}{msg}了你的请求",
            };
            sender.SendResponse();
        }
    }
}
