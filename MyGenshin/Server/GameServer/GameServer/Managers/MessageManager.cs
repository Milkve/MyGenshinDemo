using Common.Utils;
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
    class MessageManager : IPostProcess

    {
        Character Owner;
        private bool Dirty;
        Dictionary<int, NMessageInfo> AllMessages = new Dictionary<int, NMessageInfo>();
        public MessageManager(Character character)
        {

            this.Owner = character;
            MessageInit();
        }


        public void MessageInit()
        {
            AllMessages.Clear();

            foreach (var Tmessage in Owner.Data.Messages.Where(x=>x.Status<2))
            {
                AllMessages.Add(Tmessage.Id, GetMessageInfo(Tmessage));
            }
        }
        public void GetMessageInfos(List<NMessageInfo> messages)
        {

            messages.AddRange(AllMessages.Select(x => x.Value));

        }

        private NMessageInfo GetMessageInfo(TMessage tmessage)
        {
            NMessageInfo info = new NMessageInfo()
            {
                Id = tmessage.Id,
                Type = (MessageType)tmessage.Type,
                Status = tmessage.Status,
                Title = tmessage.Title,
                Message = tmessage.Message,
                Gold = tmessage.Gold,
                Exp = tmessage.Exp,

            };
            info.FromInfo = GetMessageCharInfo(tmessage.FromID);
            GetBinary2Items(info.Items, tmessage.Items);
            GetBinary2Equips(info.Equips, tmessage.Equips);

            return info;
        }

        private static void GetBinary2Equips(List<NEquipInfo> equips, byte[] bequips)
        {
            for (int i = 0; i < bequips.Length / sizeof(int); i++)
            {
                int equipId = PointerUtil.ReadInt(bequips, i);
                if (equipId != 0)
                {
                    var equipInfo = EquipManager.GetEquipInfo(equipId);
                    if (equipInfo != null)
                    {
                        equips.Add(equipInfo);
                    }
                }
            }
        }
        private static void GetEquips2Binary(List<NEquipInfo> equips, byte[] bequips)
        {
            for (int i = 0; i < equips.Count; i++)
            {
                PointerUtil.WriteInt(bequips, i, equips[i].Id);
            }
        }
        private static void GetBinary2Items(List<NItemInfo> items, byte[] bitems)
        {
            for (int i = 0; i < bitems.Length / 2 / sizeof(int); i++)
            {
                var kvp = PointerUtil.ReadKIntVInt(bitems, i);
                items.Add(new NItemInfo() { Id = kvp.Key, Count = kvp.Value });
            }
        }

        internal Result OnMessageAccept(NetConnection<NetSession> sender, MessageAcceptRequest message)
        {
            NMessageInfo nMessageInfo;
            if (AllMessages.TryGetValue(message.Id, out nMessageInfo))
            {
                if (nMessageInfo.Type == MessageType.Friend)
                {
                    if (Result.Success == FriendManager.AddFriend(sender, Owner.Id, nMessageInfo.FromInfo.Id))
                    {
                        SetMessageState(message.Id, 2);
                        return Result.Success;
                    }
                    return Result.Failed;
                }
                else if (nMessageInfo.Type == MessageType.Mail)
                {
                    //TODO:邮件接受
                }
                else if (nMessageInfo.Type == MessageType.Global)
                {

                }

            }


            sender.Session.Response.messageAcceptResponse.Errormsg = "找不到消息";
            return Result.Failed;
        }

        private void SetMessageState(int id, int v)
        {
            var tmes = Owner.Data.Messages.Where(x => x.Id == id).FirstOrDefault();
            tmes.Status = v;
        }

        private static void GetItems2Binary(List<NItemInfo> items, byte[] bitems)
        {
            for (int i = 0; i < items.Count; i++)
            {
                PointerUtil.WriteKIntVInt(bitems, i, items[i].Id, items[i].Count);
            }
        }

        public static NMessageCharInfo GetMessageCharInfo(int charid)
        {
            //如果在线
            Character character; ;
            if (CharacterManager.Instance.GetCharacter(charid, out character))
            {
                return new NMessageCharInfo()
                {
                    Id = character.Id,
                    Class = character.Data.Class,
                    Level = character.Data.Level,
                    Name = character.Data.Name
                };

            }
            var tchar = DBService.Instance.Entities.Characters.Where(x => x.ID == charid).FirstOrDefault();
            if (tchar != null) return new NMessageCharInfo()
            {
                Id = tchar.ID,
                Class = tchar.Class,
                Level = tchar.Level,
                Name = tchar.Name
            };
            return default;
        }

        internal Result OnMessageSend(NetConnection<NetSession> sender, MessageSendRequest message)
        {

            var info = GetMessageCharInfo(message.ToId);
            if (info == null)
            {
                sender.Session.Response.messageSendResponse.Errormsg = "找不到玩家";
                return Result.Failed;
            }
            TMessage tmessage;
            switch (message.Type)
            {
                case MessageType.Friend:
                    if (Owner.friendManager.HasFriend(message.ToId))
                    {
                        sender.Session.Response.messageSendResponse.Errormsg = "已经是好友";
                        return Result.Failed;
                    }
                    tmessage = GetTMessage(Owner.Id, message.ToId);
                    break;

                case MessageType.Mail:
                    tmessage = GetTMessage(message.messageInfo, Owner.Id, message.ToId);
                    break;

                default: return Result.Failed;
            }

            tmessage.Status = 0;
            DBService.Instance.Entities.Messages.Add(tmessage);
            //如果目标在线
            Character target;
            if (CharacterManager.Instance.GetCharacter(message.ToId, out target))
            {
                target.messageManager.SetDirty();
            }

            return Result.Success;
        }

        private static TMessage GetTMessage(NMessageInfo messageInfo, int id, int toId)
        {
            TMessage tmessage = new TMessage()
            {
                Id = messageInfo.Id,
                Type = (int)messageInfo.Type,
                Title = messageInfo.Title,
                Message = messageInfo.Message,
                Gold = messageInfo.Gold,
                Exp = messageInfo.Exp,
            };
            tmessage.Items = new byte[sizeof(int) * 20];
            GetItems2Binary(messageInfo.Items, tmessage.Items);
            tmessage.Equips = new byte[sizeof(int) * 10];
            GetEquips2Binary(messageInfo.Equips, tmessage.Equips);
            tmessage.FromID = id;
            tmessage.TCharacterID = toId;
            return tmessage;
        }
        private static TMessage GetTMessage(int id, int toId)
        {
            TMessage tmessage = new TMessage();
            tmessage.Type = (int)MessageType.Friend;
            tmessage.FromID = id;
            tmessage.TCharacterID = toId;
            return tmessage;
        }

        public void SetDirty()
        {
            Dirty = true;
        }
        public void PostProcess(NetMessage message)
        {
            if (Dirty)
            {
                if (message.Response.messageListResponse == null)
                {
                    MessageInit();
                    message.Response.messageListResponse = new MessageListResponse();
                    message.Response.messageListResponse.Messages.AddRange(AllMessages.Select(x => x.Value));
                    Dirty = false;
                }
            }
        }
    }
}
