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
            MessageDistributer<NetConnection<NetSession>>.Instance.Subscribe<MessageReplyRequest>(this.OnMessageReply);
            MessageDistributer<NetConnection<NetSession>>.Instance.Subscribe<MessageListRequest>(this.OnMessageList);
            MessageDistributer<NetConnection<NetSession>>.Instance.Subscribe<MessageTargetInfoRequest>(this.OnMessageTargetInfo);
            MessageDistributer<NetConnection<NetSession>>.Instance.Subscribe<FriendListRequest>(this.OnFriendList);
            MessageDistributer<NetConnection<NetSession>>.Instance.Subscribe<FriendRemoveRequest>(this.OnFriendRemove);

        }

        private void OnMessageList(NetConnection<NetSession> sender, MessageListRequest message)
        {
            sender.Session.Character.messageManager.SetDirty();
            sender.Session.Response.Update = new StatusUpdate();
            sender.SendResponse();
        }

        private void OnMessageTargetInfo(NetConnection<NetSession> sender, MessageTargetInfoRequest message)
        {
            var info = MessageManager.GetMessageCharInfo(message.Id);
            sender.Session.Response.messageTargetInfoResponse = new MessageTargetInfoResponse()
            {
                Result = info != null ? Result.Success : Result.Failed,
                Info = info,
                Type = message.Type,
                Errormsg = info != null ? string.Empty : "找不到玩家",
            };
            sender.SendResponse();
        }

        private void OnFriendList(NetConnection<NetSession> sender, FriendListRequest message)
        {

            sender.Session.Character.friendManager.SetDirty();
            sender.Session.Response.Update = new StatusUpdate();
            sender.SendResponse();
        }

        private void OnFriendRemove(NetConnection<NetSession> sender, FriendRemoveRequest message)
        {
            Result res = FriendManager.RemoveFriend(sender.Session.Character.Id, message.friendId);
            if (res == Result.Success)
            {
                DBService.Instance.Save();
                FriendManager.FriendRemoveNoisy(sender.Session.Character.Id);
                FriendManager.FriendRemoveNoisy(message.friendId);
                
            }
            sender.Session.Response.friendRemove = new FriendRemoveResponse()
            {
                Result = res,
            };
            sender.SendResponse();
        }


        /// <summary>
        /// 消息回复响应
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="message"></param>
        private void OnMessageReply(NetConnection<NetSession> sender, MessageReplyRequest message)
        {
            sender.Session.Response.messageReplyResponse = new MessageReplyResponse();
            MessageManager messageManager = sender.Session.Character.messageManager;
            NMessageInfo nMessageInfo;
            if (!messageManager.HasManager(message.Id, out nMessageInfo, message.Type))
            {
                sender.Session.Response.messageReplyResponse.Errormsg = "找不到消息";
                sender.Session.Response.messageReplyResponse.Result = Result.Failed;
                sender.SendResponse();
                return;
            }
            int accept = sender.Session.Character.Id;  //message的接收者
            int send = nMessageInfo.FromInfo.Id;
            NMessageCharInfo acceptInfo = MessageManager.GetMessageCharInfo(accept);
            NMessageCharInfo sendInfo = MessageManager.GetMessageCharInfo(send);
            if (nMessageInfo.Type == MessageType.Friend)
            {
                if (message.Reply == MessageReply.Accept)
                {
                    if (FriendManager.HasFriend(accept, send) || FriendManager.HasFriend(send, accept))
                    {
                        sender.Session.Response.messageReplyResponse.Errormsg = "已存在的好友";
                        sender.Session.Response.messageReplyResponse.Result = Result.Failed;
                        sender.SendResponse();
                        return;
                    }

                    FriendManager.AddFriend(accept, send, acceptInfo, sendInfo);
                    messageManager.SetMessageState(message.Id, nMessageInfo.Type, 2);
                    sender.Session.Response.messageReplyResponse.Result = Result.Success;
                    sender.SendResponse();
                    FriendManager.FriendAddNoisy(send, acceptInfo, Result.Success, true);//添加好友成功
                    FriendManager.FriendAddNoisy(accept, sendInfo, Result.Success, false);
                    return;
                }
                else if (message.Reply == MessageReply.Refuse)
                {
                    messageManager.SetMessageState(message.Id, nMessageInfo.Type, 2);
                    sender.Session.Response.messageReplyResponse.Result = Result.Success;
                    sender.SendResponse();
                    FriendManager.FriendAddNoisy(send, acceptInfo, Result.Failed, true);
                    FriendManager.FriendAddNoisy(accept, sendInfo, Result.Failed, false);
                    return;
                }
            }
            else if (nMessageInfo.Type == MessageType.Mail)
            {
                //TODO:邮件接受
            }
            else if (nMessageInfo.Type == MessageType.Global)
            {
                if (message.Reply == MessageReply.Accept &&nMessageInfo.Status==0)
                {
                  
                    foreach (var item in nMessageInfo.Items)
                    {
                        sender.Session.Character.itemManager.AddItem(item.Id, item.Count);
                    }
                    if (nMessageInfo.Gold > 0)
                    {
                        sender.Session.Character.Gold += nMessageInfo.Gold;
                    }
                    messageManager.SetMessageState(message.Id, nMessageInfo.Type, 1);
                    DBService.Instance.Save(false);
                    sender.Session.Response.messageReplyResponse.Result = Result.Success;
                    sender.SendResponse();
                }
                else if (message.Reply == MessageReply.Delete && nMessageInfo.Status == 1)
                {
                    messageManager.SetMessageState(message.Id, nMessageInfo.Type, 2);
                    DBService.Instance.Save(false);
                    sender.Session.Response.messageReplyResponse.Result = Result.Success;
                    sender.SendResponse();
                }
            }
        }

        private void OnMessageSend(NetConnection<NetSession> sender, MessageSendRequest message)
        {

            sender.Session.Response.messageSendResponse = new MessageSendResponse();
            Result res = sender.Session.Character.messageManager.OnMessageSend(sender, message);
            Log.Info($@"OnMessageSend Type:{message.Type} 
                        From:{message.messageInfo.FromInfo.Id} To:{message.ToId} 
                        Result:{res}
                        Msg{sender.Session.Response.messageSendResponse.Errormsg}");
            sender.Session.Response.messageSendResponse.Type = message.Type;
            sender.Session.Response.messageSendResponse.Result = res;
            sender.SendResponse();
        }

        public void SendMessageReceive(NetConnection<NetSession> sender)
        {
            sender.Session.Response.messageReceive = new MessageReceive();
            sender.SendResponse();
        }

        public void SendAddFriendResponse(NetConnection<NetSession> sender, Result result, string msg)
        {
            sender.Session.Response.friendAdd = new FriendAddResponse()
            {
                Result = result,
                Errormsg = msg,
            };
            sender.SendResponse();
        }


        public void SendUpdate(NetConnection<NetSession> sender)
        {
            sender.Session.Response.Update = new StatusUpdate();
            sender.SendResponse();
        }
    }
}
