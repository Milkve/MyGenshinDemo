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
            Log.Info($"Character{ Owner.Id} FriendInit");
            AllFriends.Clear();
            foreach (var friend in Owner.Data.Friends.Where(x=>x.IsDelete==false))
            {
                AllFriends.Add(friend.FriendID, GetFriendInfo(friend));
            }
        }

        public bool HasFriend(int friendID)
        {
            return AllFriends.ContainsKey(friendID);
        }

        public static bool HasFriend(int OwnerID, int friendID)
        {
            return DBService.Instance.Entities.Friends.Where(x => x.FriendID == friendID && x.TCharacterID == OwnerID &&x.IsDelete==false).Count() != 0;
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

        public static TFriend GetTFriend(int ownerID, NMessageCharInfo friendInfo)
        {
            TFriend friend = new TFriend()
            {
                FriendID = friendInfo.Id,
                TCharacterID = ownerID,
                FriendClass = friendInfo.Class,
                FriendLevel = friendInfo.Level,
                FriendName = friendInfo.Name,
            };
            return friend;
        }


        //public void SetFriendInfo(int friendID,int status)
        //{
        //    if (AllFriends.ContainsKey(friendID))
        //    {
        //        AllFriends[friendID].Status = status;
        //        SetDirty();
        //    }
        //}

        internal void OfflineNoisy()
        {
            foreach (var friend in AllFriends)
            {
                if (friend.Value.Status == 1)
                {
                    NetConnection<NetSession> session;
                    if(SessionManager.Instance.GetSession(friend.Value.friendInfo.Id, out session))
                    {
                        session.Session.Character.friendManager.SetDirty();
                        MessageService.Instance.SendUpdate(session);
                    }
                }
            }
        }

        internal static void AddFriend(int id1, int id2, NMessageCharInfo info1, NMessageCharInfo info2)
        {

            TFriend friend1 = DBService.Instance.Entities.Friends.Where(x => x.FriendID == id2 && x.TCharacterID == id1).FirstOrDefault();
            if (friend1 == null)
            {
                friend1 = GetTFriend(id1, info2);
                DBService.Instance.Entities.Friends.Add(friend1);
            }
            else
            {
                friend1.IsDelete = false;
            }
            TFriend friend2 = DBService.Instance.Entities.Friends.Where(x => x.FriendID == id1 && x.TCharacterID == id2).FirstOrDefault();
            if (friend2 == null)
            {
                friend2 = GetTFriend(id2, info1);
                DBService.Instance.Entities.Friends.Add(friend2);
            }
            else
            {
                friend2.IsDelete = false;
            }


        }

        internal static Result RemoveFriend(int id, int friendId)
        {
            try
            {
                TFriend friendship1 = DBService.Instance.Entities.Friends.Where(x => x.TCharacterID == id && x.FriendID == friendId).First();
                friendship1.IsDelete = true;               
                TFriend friendship2 = DBService.Instance.Entities.Friends.Where(x => x.TCharacterID == friendId && x.FriendID == id).First();
                friendship2.IsDelete = true;
                return Result.Success;
            }
            catch (Exception e)
            {
                Log.Error(e);
                return Result.Failed;
            }
        }


        public static void FriendAddNoisy(int owner, NMessageCharInfo info, Result result, bool isSender)
        {
            NetConnection<NetSession> session;
            if (SessionManager.Instance.GetSession(owner, out session))
            {
                Log.Info($@"OwnerID:{owner} FriendID{info.Id} Reuslt:{result} IsSender:{isSender}");
                session.Session.Character.friendManager.SetDirty();
                string msg;
                if (isSender && result == Result.Success)
                {
                    msg = $"玩家[{info.Name}](UID:{info.Id})接受了你的好友请求";
                }
                else if (isSender && result == Result.Failed)
                {
                    msg = $"玩家[{info.Name}](UID:{info.Id})拒绝了你的好友请求";
                }
                else if (!isSender && result == Result.Success)
                {
                    msg = $"已玩家[{info.Name}](UID:{info.Id})成为好友";
                }
                else
                {
                    msg = $"已拒绝玩家[{info.Name}](UID:{info.Id})的好友请求";
                }
                MessageService.Instance.SendAddFriendResponse(session, result, msg);

            }
        }
        public static void FriendRemoveNoisy(int owner)
        {
            NetConnection<NetSession> session;
            if (SessionManager.Instance.GetSession(owner, out session))
            {
                session.Session.Character.friendManager.SetDirty();
            }
        }
        public void SetDirty()
        {
            Dirty = true;
        }
        public void PostProcess(NetMessage message)
        {
            if (Dirty)
            {
                if (message.Response.friendList == null )
                {
                    FriendInit();
                    message.Response.friendList = new FriendListResponse();
                    message.Response.friendList.Friends.AddRange(AllFriends.Select(x => x.Value));
                    message.Response.friendList.Result = Result.Success;
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
            DBService.Instance.Save();


            
        }
    }
}
