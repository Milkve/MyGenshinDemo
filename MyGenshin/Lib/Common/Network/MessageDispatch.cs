//WARNING: DON'T EDIT THIS FILE!!!
using Common;

namespace Network
{
    public class MessageDispatch<T> : Singleton<MessageDispatch<T>>
    {
        public void Dispatch(T sender, SkillBridge.Message.NetMessageResponse message)
        {
            if (message.userRegister != null) { MessageDistributer<T>.Instance.RaiseEvent(sender, message.userRegister); }
            if (message.userLogin != null) { MessageDistributer<T>.Instance.RaiseEvent(sender, message.userLogin); }
            if (message.createChar != null) { MessageDistributer<T>.Instance.RaiseEvent(sender, message.createChar); }
            if (message.gameEnter != null) { MessageDistributer<T>.Instance.RaiseEvent(sender, message.gameEnter); }
            if (message.gameLeave != null) { MessageDistributer<T>.Instance.RaiseEvent(sender, message.gameLeave); }
            if (message.mapCharacterEnter != null) { MessageDistributer<T>.Instance.RaiseEvent(sender, message.mapCharacterEnter); }
            if (message.mapCharacterLeave != null) { MessageDistributer<T>.Instance.RaiseEvent(sender, message.mapCharacterLeave); }
            if (message.mapEntitySync != null) { MessageDistributer<T>.Instance.RaiseEvent(sender, message.mapEntitySync); }
            if (message.itemBuyResponse != null) { MessageDistributer<T>.Instance.RaiseEvent(sender, message.itemBuyResponse); }
            if (message.itemSellResponse != null) { MessageDistributer<T>.Instance.RaiseEvent(sender, message.itemSellResponse); }
            if (message.statusNotify != null) { MessageDistributer<T>.Instance.RaiseEvent(sender, message.statusNotify); }
            if (message.equipResponse!= null) { MessageDistributer<T>.Instance.RaiseEvent(sender, message.equipResponse); }
            if (message.questAcceptResponse != null) { MessageDistributer<T>.Instance.RaiseEvent(sender, message.questAcceptResponse); }
            if (message.questListResponse!= null) { MessageDistributer<T>.Instance.RaiseEvent(sender, message.questListResponse); }
            if (message.questSubmitResponse != null) { MessageDistributer<T>.Instance.RaiseEvent(sender, message.questSubmitResponse); }
        }

        public void Dispatch(T sender, SkillBridge.Message.NetMessageRequest message)
        {
            if (message.userRegister != null) { MessageDistributer<T>.Instance.RaiseEvent(sender,message.userRegister); }
            if (message.userLogin != null) { MessageDistributer<T>.Instance.RaiseEvent(sender, message.userLogin); }
            if (message.createChar != null) { MessageDistributer<T>.Instance.RaiseEvent(sender, message.createChar); }
            if (message.gameEnter != null) { MessageDistributer<T>.Instance.RaiseEvent(sender, message.gameEnter); }
            if (message.gameLeave != null) { MessageDistributer<T>.Instance.RaiseEvent(sender, message.gameLeave); }
            if (message.mapCharacterEnter != null) { MessageDistributer<T>.Instance.RaiseEvent(sender, message.mapCharacterEnter); }
            if (message.mapEntitySync != null) { MessageDistributer<T>.Instance.RaiseEvent(sender, message.mapEntitySync); }
            if (message.mapTeleport != null) { MessageDistributer<T>.Instance.RaiseEvent(sender, message.mapTeleport); }
            if (message.itemBuyRequest != null) { MessageDistributer<T>.Instance.RaiseEvent(sender, message.itemBuyRequest); }
            if (message.itemSellRequest != null) { MessageDistributer<T>.Instance.RaiseEvent(sender, message.itemSellRequest); }
            if (message.equipRequest!= null) { MessageDistributer<T>.Instance.RaiseEvent(sender, message.equipRequest); }
            if (message.questAcceptRequest != null) { MessageDistributer<T>.Instance.RaiseEvent(sender, message.questAcceptRequest); }
            if (message.questListRequest != null) { MessageDistributer<T>.Instance.RaiseEvent(sender, message.questListRequest); }
            if (message.questSubmitRequest != null) { MessageDistributer<T>.Instance.RaiseEvent(sender, message.questSubmitRequest); }
        }
    }
}