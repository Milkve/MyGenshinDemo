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

        public List<NFriendInfo> AllFriends;


        public void Init(List<NFriendInfo> friendInfos)
        {
            AllFriends.AddRange(friendInfos);
            
        }

        internal void SendAddFriend(int friendId)
        {
            FriendService.Instance.SendFriendAddRequest(friendId);
        }

        public void Show()
        {
            foreach (var item in AllFriends)
            {
                Debug.Log(item.friendInfo.Name);
            }
        }


    }
}
