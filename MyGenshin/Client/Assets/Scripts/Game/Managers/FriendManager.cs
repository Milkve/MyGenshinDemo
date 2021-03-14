using Entities;
using Services;
using SkillBridge.Message;
using System;
using System.Collections.Generic;

using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Managers
{
    class FriendManager : Singleton<FriendManager>
    {
        public EventWraperV1<MessageTargetInfoResponse> OnTargetInfo = new EventWraperV1<MessageTargetInfoResponse>();
        public EventWraper OnFriendUpdate                            = new EventWraper();
        public EventWraperV1<string> OnFriendAddResponse  = new EventWraperV1<string>();
        public EventWraperV1<Result> OnFriendRemoveResponse = new EventWraperV1<Result>();

        public Dictionary<int, NFriendInfo> AllFriends = new Dictionary<int, NFriendInfo>();
        internal void FriendInit(NCharacterInfo character)
        {
            FriendInit(character.Friends);
        }

        public void FriendInit(List<NFriendInfo> friends)
        {
            AllFriends.Clear();
            foreach (var friend in friends)
            {
                AllFriends.Add(friend.friendInfo.Id, friend);
            }
            Debug.Log($"OnFriendUpdate");
            OnFriendUpdate.Invoke();
        }

        public bool IsFriend(int id)
        {
            return AllFriends.ContainsKey(id);
        }
        public void SendAddFriend(int friendId)
        {
            FriendService.Instance.SendFriendAddRequest(friendId);
        }


        public void SendDeleteFriend(int friendID)
        {
            FriendService.Instance.SendFriendDeleteRequest(friendID);
        }

        internal void ShowTargetInfo(MessageTargetInfoResponse message)
        {
            OnTargetInfo?.Invoke(message);
        }

        public List<NFriendInfo> GetFriends()
        {
            return AllFriends.Select(x => x.Value).OrderByDescending(x => x.Status).ToList();
        }

        internal void OnFriendAdd(FriendAddResponse message)
        {
            OnFriendAddResponse.Invoke(message.Errormsg);
        }

        internal void FriendRemove(Result result)
        {
            OnFriendRemoveResponse.Invoke(result);
        }
    }
}
