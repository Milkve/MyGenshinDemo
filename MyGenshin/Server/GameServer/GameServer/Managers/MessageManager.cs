using Common;
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
        List<NMessageInfo> AllMessages = new List<NMessageInfo>();
        public MessageManager(Character character)
        {

            this.Owner = character;
            MessageInit();
        }


        public void MessageInit()
        {
            AllMessages.Clear();

            foreach (var Tmessage in Owner.Data.Messages.Where(x => x.Status < 2))
            {

                AllMessages.Add(GetMessageInfo(Tmessage));
            }
            UpdateGloBalMessage();
            foreach (var TGlobalMessageStatus in Owner.Data.CharacterGlobalStatus.Where(x => x.Status < 2))
            {
                TGlobalMessage message = DBService.Instance.Entities.GlobalMessages.Where(x => x.Id == TGlobalMessageStatus.MessageID).FirstOrDefault();
                AllMessages.Add(GetMessageInfo(message, TGlobalMessageStatus.Status));
            }

        }


        public void UpdateGloBalMessage()
        {
            int count = DBService.Instance.Entities.GlobalMessages.Count();
            while (Owner.Data.CGMsgID <count )
            {
                Owner.Data.CGMsgID += 1;
                TCharacterGlobalStatus status = new TCharacterGlobalStatus()
                {
                    MessageID = Owner.Data.CGMsgID,
                    Status = 0
                };
                Owner.Data.CharacterGlobalStatus.Add(status);
            }
            DBService.Instance.Save(false);
        }
        public void GetMessageInfos(List<NMessageInfo> messages)
        {

            messages.AddRange(AllMessages);

        }

        public static void AddGolbalTest()
        {

            NMessageInfo nMessageInfo = new NMessageInfo()
            {
                Title = "新手福利",
                Message = "这是新手福利",
                Exp = 200,
                Gold = 10000,

            };
            nMessageInfo.Items.Add(new NItemInfo() { Id = 1, Count = 10 });
            nMessageInfo.Items.Add(new NItemInfo() { Id = 2, Count = 10 });
            TGlobalMessage tmessage = GetTGlobalMessage(nMessageInfo, 0);
            DBService.Instance.Entities.GlobalMessages.Add(tmessage);
            DBService.Instance.Save(false);

            SessionManager.Instance.ForeachSession(session =>
            {
                session.Session.Character.messageManager.SetDirty();
                MessageService.Instance.SendMessageReceive(session);
            });
            Log.Info("SendGlobalCompleted");
        }

        private NMessageInfo GetMessageInfo(TMessage tmessage)
        {
            NMessageInfo info = new NMessageInfo()
            {
                Id = tmessage.Id,
                Type = (MessageType)tmessage.Type,
                Status = tmessage.Status,
                Title = tmessage.Title ?? "",
                Message = tmessage.Message ?? "",
                Gold = tmessage.Gold ?? 0,
                Exp = tmessage.Exp ?? 0,
                Time = tmessage.Time

            };
            info.FromInfo = GetMessageCharInfo(tmessage.FromID);
            GetBinary2Items(info.Items, tmessage.Items ?? new byte[0]);
            GetBinary2Equips(info.Equips, tmessage.Equips ?? new byte[0]);

            return info;
        }
        private NMessageInfo GetMessageInfo(TGlobalMessage tmessage, int Status)
        {
            NMessageInfo info = new NMessageInfo()
            {
                Id = tmessage.Id,
                Type = MessageType.Global,
                Status = Status,
                Title = tmessage.Title ?? "",
                Message = tmessage.Message ?? "",
                Gold = tmessage.Gold ?? 0,
                Exp = tmessage.Exp ?? 0,
                Time = tmessage.Time

            };
            info.FromInfo = GetMessageCharInfo(tmessage.FromID, MessageType.Global);
            GetBinary2Items(info.Items, tmessage.Items ?? new byte[0]);
            GetBinary2Equips(info.Equips, tmessage.Equips ?? new byte[0]);

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
                if (kvp.Key != 0 && kvp.Value != 0)
                {
                    items.Add(new NItemInfo() { Id = kvp.Key, Count = kvp.Value });
                }

            }
        }

        private static void GetItems2Binary(List<NItemInfo> items, byte[] bitems)
        {
            for (int i = 0; i < items.Count; i++)
            {
                PointerUtil.WriteKIntVInt(bitems, i, items[i].Id, items[i].Count);
            }
        }
        public bool HasManager(int Id, out NMessageInfo nMessageInfo, MessageType type)
        {

            nMessageInfo = AllMessages.Where(x => x.Id == Id && x.Type == type).FirstOrDefault();

            if (nMessageInfo == null)
            {
                return false;
            }
            return true;
        }


        public void SetMessageState(int id, MessageType type, int status)
        {
            if (type == MessageType.Global)
            {
                Owner.Data.CharacterGlobalStatus.Where(x => x.MessageID == id).FirstOrDefault().Status = status;
            }
            else
            {
                Owner.Data.Messages.Where(x => x.Id == id).FirstOrDefault().Status = status;
            }

            SetDirty();
        }



        public static NMessageCharInfo GetMessageCharInfo(int charid, MessageType type = MessageType.Friend)
        {
            if (type == MessageType.Friend || type == MessageType.Mail)
            {
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
            }
            else
            {
                if (charid == 0)
                {
                    return new NMessageCharInfo()
                    {
                        Id = 0,
                        Name = "系统管理员"
                    };
                }
            }

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
                    if (IsExsistFriendApply(message.ToId, sender.Session.Character.Id))
                    {
                        sender.Session.Response.messageSendResponse.Errormsg = "已经在申请列表了";
                        return Result.Failed;
                    }
                    if (Owner.Id == message.ToId)
                    {
                        sender.Session.Response.messageSendResponse.Errormsg = "不能添加自己";
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
            DBService.Instance.Entities.Characters.Where(x => x.ID == message.ToId).FirstOrDefault().Messages.Add(tmessage);
            DBService.Instance.Save(false);
            Log.Info($"tmessage{tmessage.Id}");
            //如果目标在线

            NetConnection<NetSession> session;
            if (SessionManager.Instance.GetSession(message.ToId, out session))
            {

                session.Session.Character.messageManager.SetDirty();
                MessageService.Instance.SendMessageReceive(session);
            }




            return Result.Success;
        }

        private static bool IsExsistFriendApply(int toId, int id)
        {
            Log.Info($"IsExsist to{toId} from{id}");
            NetConnection<NetSession> session;
            if (SessionManager.Instance.GetSession(toId, out session))
            {
                return session.Session.Character.messageManager.AllMessages
                    .Where(x => x.FromInfo.Id == id).Count() != 0;
            }
            return DBService.Instance.Entities.Messages
                .Where(x => x.FromID == id && x.Type == (int)MessageType.Friend && x.TCharacterID == toId).Count() != 0;

        }


        private static TMessage GetTMessage(NMessageInfo messageInfo, int id, int toId)
        {
            TMessage tmessage = new TMessage()
            {
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
            tmessage.Time = Time.GetTimeStamp();
            return tmessage;
        }
        private static TGlobalMessage GetTGlobalMessage(NMessageInfo messageInfo, int id)
        {
            TGlobalMessage tmessage = new TGlobalMessage()
            {
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
            tmessage.Time = Time.GetTimeStamp();
            return tmessage;
        }
        private static TMessage GetTMessage(int id, int toId)
        {
            TMessage tmessage = new TMessage();
            tmessage.Type = (int)MessageType.Friend;
            tmessage.FromID = id;
            //tmessage.TCharacterID = toId;
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
                    message.Response.messageListResponse.Messages.AddRange(AllMessages);
                    Dirty = false;
                }
            }
        }
    }
}
