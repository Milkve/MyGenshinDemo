using Common;
using GameServer.Entities;
using GameServer.Services;
using Interface;
using Network;
using SkillBridge.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Managers
{
    class FriendManager : IPostProcess
    {
        Dictionary<int, NFriendInfo> AllFriends = new Dictionary<int, NFriendInfo>();
        Character Owner;

        bool Dirty;
        public FriendManager(Character character)
        {
            Owner = character;
            FriendInit();
        }

        private void FriendInit()
        {
            AllFriends.Clear();
            foreach (var friend in Owner.Data.Friends)
            {
                AllFriends.Add(friend.FriendID, GetFriendInfo(friend));
            }
        }

        public bool HasFriend(int friendID)
        {
            return AllFriends.ContainsKey(friendID);
        }
        public void GetFriendsInfo(List<NFriendInfo> Friends)
        {
            foreach (var friend in AllFriends)
            {
                Friends.Add(friend.Value);
            }
        }



        private NFriendInfo GetFriendInfo(TFriend tfriend)
        {

            Character friend;
            NFriendInfo nFriendInfo = new NFriendInfo();
            nFriendInfo.Id = tfriend.Id;
            if (!CharacterManager.Instance.GetCharacter(tfriend.FriendID, out friend))
            { //好友不在线
                nFriendInfo.friendInfo = new NMessageCharInfo()
                {
                    Id = tfriend.FriendID,
                    Name = tfriend.FriendName,
                    Level = tfriend.FriendLevel,
                    Class = tfriend.FriendClass,

                };
                nFriendInfo.Status = 0;
            }
            else
            {//好友在线
                nFriendInfo.friendInfo = new NMessageCharInfo()
                {
                    Id = friend.Id,
                    Name = friend.Data.Name,
                    Level = friend.Data.Level,
                    Class = friend.Data.Class,
                };
                nFriendInfo.Status = 1;
                friend.friendManager.UpdateFriendInfo(Owner.Info, 1);
            }
            return nFriendInfo;
        }

        private void UpdateFriendInfo(NCharacterInfo info, int status)
        {
            if (AllFriends.ContainsKey(info.Id))
            {
                AllFriends[info.Id].Status = status;
            }
            Dirty = true;
        }


        internal static Result AddFriend(NetConnection<NetSession> sender, int id1, int id2)
        {

            Result res = Result.Success;
            try
            {
                NMessageCharInfo info1 = MessageManager.GetMessageCharInfo(id1);
                TFriend friend1 = new TFriend()
                {
                    FriendID = id1,
                    TCharacterID = id2,
                    FriendClass = info1.Class,
                    FriendLevel = info1.Level,
                    FriendName = info1.Name,
                };
               

                NMessageCharInfo info2 = MessageManager.GetMessageCharInfo(id2);
                TFriend friend2 = new TFriend()
                {
                    FriendID = id2,
                    TCharacterID = id1,
                    FriendClass = info2.Class,
                    FriendLevel = info2.Level,
                    FriendName = info2.Name,
                };
                DBService.Instance.Entities.Friends.Add(friend1);
                DBService.Instance.Entities.Friends.Add(friend2);
                NetConnection<NetSession> session;
                if (SessionManager.Instance.GetSession(id1, out session)){
                    session.Session.Character.friendManager.SetDirty();
                    MessageService.Instance.SendAddFriendResponse(session, Result.Success, info1);
                }
                if (SessionManager.Instance.GetSession(id2, out session))
                {
                    session.Session.Character.friendManager.SetDirty();
                    MessageService.Instance.SendAddFriendResponse(session, Result.Success, info2);
                }
            }
            catch (Exception e)
            {
                sender.Session.Response.messageAcceptResponse.Errormsg = e.Message;
                res = Result.Failed;
            }
            return res;
        }



        public void SetDirty()
        {
            Dirty = true;
        }
        public void PostProcess(NetMessage message)
        {
            if (Dirty)
            {
                if (message.Response.friendList == null)
                {
                    FriendInit();
                    message.Response.friendList = new FriendListResponse();
                    message.Response.friendList.Friends.AddRange(AllFriends.Select(x => x.Value));
                    Dirty = false;
                }
            }
        }

        internal void UpdateInfo()
        {
            foreach (var friendship in DBService.Instance.Entities.Friends.Where(x => x.FriendID == Owner.Id))
            {
                friendship.FriendLevel = Owner.Data.Level;
                friendship.FriendName = Owner.Data.Name;
                friendship.FriendClass = Owner.Data.Class;
            }
        }
    }
}
