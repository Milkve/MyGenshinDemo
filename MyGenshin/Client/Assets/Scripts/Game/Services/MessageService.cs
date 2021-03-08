using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Network;
using SkillBridge.Message;
using UnityEngine;

namespace Services
{
    class MessageService : Singleton<MessageService>, IDisposable
    {

        public MessageService()
        {
            MessageDistributer.Instance.Subscribe<MessageListResponse>(this.OnMessageList);



        }

        private void OnMessageList(object sender, MessageListResponse message)
        {
            foreach (var item in message.Messages)
            {

            }  
        }

        public void Dispose()
        {

        }


        public void SendFriendAddRequest(int friendID)
        {
            NetMessage message = new NetMessage()
            {
                Request = new NetMessageRequest()
                {
                    messageSendRequest=new MessageSendRequest()
                    {
                        Type=MessageType.Friend,
                        ToId=friendID,
                        messageInfo=new NMessageInfo()
                        {
                            FromInfo =new NMessageCharInfo()
                            {
                                Id=Models.User.Instance.CurrentCharacter.Id,
                                Name= Models.User.Instance.CurrentCharacter.Name,
                                Level= Models.User.Instance.CurrentCharacter.Level,
                                Class= Models.User.Instance.CurrentCharacter.Class
                            }
                        }
                    }
                }
            };
            NetClient.Instance.SendMessage(message);
        }




        //public void SendFriendAddResponse(bool accept, FriendAddRequest request)
        //{
        //    NetMessage message = new NetMessage()
        //    {
        //        Request = new NetMessageRequest()
        //        {
        //            friendAddResponse = new FriendAddResponse()
        //            {
        //                Result = accept ? Result.Success : Result.Failed,
        //                Errormsg = accept ? "对方同意了你的好友申请" : "对方拒绝了你的请求",
        //                Request = request
        //            }
        //        }
        //    };
        //    NetClient.Instance.SendMessage(message);
        //}

        public void SendFriendRemove(int friendID)
        {
            NetMessage message = new NetMessage()
            {
                Request = new NetMessageRequest()
                {
                    friendRemove = new FriendRemoveRequest()
                    {
                        friendId = friendID
                    }
                }
            };

        }





        private void OnFriendList(object sender, FriendListResponse message)
        {

        }

        private void OnFriendRemove(object sender, FriendRemoveResponse message)
        {

        }

        private void OnFriendAddResponse(object sender, FriendAddResponse message)
        {
            Debug.Log(message.Errormsg);
        }

        //private void OnFriendAddRequest(object sender, FriendAddRequest message)
        //{
            
        //}
    }
}
