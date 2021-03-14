using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Services;
using SkillBridge.Message;
using UnityEngine;

namespace Managers
{
    class MessageManager : Singleton<MessageManager>
    {
        public EventWraper OnMessageUpdate = new EventWraper();
        public EventWraperV1<MessageSendResponse> OnFriendMessageSend = new EventWraperV1<MessageSendResponse>();
        public EventWraper OnMessageReceive = new EventWraper();
        List< NMessageInfo> AllMessages = new List<NMessageInfo>();
        public void MessageInit(NCharacterInfo nCharacter)
        {
            MessageInit(nCharacter.Messages);
        }

        public void MessageInit(List<NMessageInfo> messages)
        {

            
            AllMessages.Clear();
            foreach (var message in messages)
            {
                Debug.Log($"Message ID:{message.Id} from:{message.FromInfo.Id}");
                AllMessages.Add( message);
            }
            Debug.Log($"OnMessageUpdated");
            OnMessageUpdate?.Invoke();
        }

        internal void MessageReceive()
        {
            OnMessageReceive.Invoke();
        }

        internal void OnMessageReply(MessageReplyResponse message)
        {
            Debug.Log($"MessageReply {message.Result} {message.Errormsg}");
        }

        public void SendMessageReply(int messageId, MessageReply reply,MessageType type)
        {

            NMessageInfo nMessageInfo=AllMessages.Where(x => x.Id == messageId && x.Type == type).FirstOrDefault();
            if (nMessageInfo!=null)
            {
                MessageService.Instance.SendMessageReply(nMessageInfo.Id, nMessageInfo.Type, reply);
            }

        }
        public void OnMessageSend( MessageSendResponse message)
        {
            Debug.Log($@"OnMessageSend Type:{message.Type} Reuslt:{message.Result}
Errormsg:{message.Errormsg}");
            if (message.Type == MessageType.Friend)
            {       
                OnFriendMessageSend.Invoke(message);
            }
        }
        internal void OnMessageList(List<NMessageInfo> messages)
        {
            throw new NotImplementedException();
        }

        public void SendMessageTarget(MessageType type,int Id,string name="")
        {
            MessageService.Instance.SendMessageTarget(type,Id, name);
        }

        public List<NMessageInfo> GetFriendMessage()
        {

            return AllMessages.Where(x => x.Type == MessageType.Friend).ToList();
        }
       
        public List<NMessageInfo> GetMailMessage()
        {

            return AllMessages.Where(x => x.Type == MessageType.Global||x.Type==MessageType.Mail).OrderBy(x => x.Id).ToList();

        }

    }
}
