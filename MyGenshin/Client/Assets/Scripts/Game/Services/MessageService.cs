using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Network;
using SkillBridge.Message;
using UnityEngine;
using Managers;

namespace Services
{
    class MessageService : Singleton<MessageService>, IDisposable
    {

        
        public MessageService()
        {
            MessageDistributer.Instance.Subscribe<MessageReplyResponse>(this.OnMessageReply);
            MessageDistributer.Instance.Subscribe<MessageSendResponse>(this.OnMessageSend);
            MessageDistributer.Instance.Subscribe<MessageListResponse>(this.OnMessageList);
            MessageDistributer.Instance.Subscribe<MessageTargetInfoResponse>(this.OnMessageTarget);

            MessageDistributer.Instance.Subscribe<MessageReceive>(this.OnMessageReceive);
        }

        private void OnMessageReceive(object sender, MessageReceive message)
        {
            MessageManager.Instance.MessageReceive();
        }

        private void OnMessageReply(object sender, MessageReplyResponse message)
        {
            MessageManager.Instance.OnMessageReply(message);
        }

        private void OnMessageSend(object sender, MessageSendResponse message)
        {
            MessageManager.Instance.OnMessageSend( message);
        }

        private void OnMessageList(object sender, MessageListResponse message)
        {
            MessageManager.Instance.MessageInit(message.Messages);
        }

        public void Dispose()
        {

        }

        internal void SendMessageReply(int id,MessageType type,MessageReply reply)
        {
            NetMessage message = new NetMessage()
            {

                Request = new NetMessageRequest()
                {
                    messageReplyRequest = new MessageReplyRequest()
                    {
                        Id = id,
                        Type = type,
                        Reply = reply
                    }
                }
            };
            NetClient.Instance.SendMessage(message);
        }

        internal void SendMessageTarget(MessageType type, int id, string name)
        {
            NetMessage message = new NetMessage()
            {

                Request = new NetMessageRequest()
                {
                    messageTargetInfoRequest = new MessageTargetInfoRequest()
                    {
                        Id = id,
                        Name = name,
                        Type = type
                    }
                }
            };
            NetClient.Instance.SendMessage(message);
        }

        private void OnMessageTarget(object sender, MessageTargetInfoResponse message)
        {


            if (message.Type == MessageType.Friend)
            {
                FriendManager.Instance.ShowTargetInfo(message);
            }
        }

    }
}
