using System;
using Network;
using SkillBridge.Message;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Managers;

namespace Services
{
    class StatusService : Singleton<StatusService>, IDisposable
    {
        public delegate bool StatusNotifyHandler(NStatus nStatus);

        Dictionary<StatusType, StatusNotifyHandler> eventMap = new Dictionary<StatusType, StatusNotifyHandler>();

        public Queue<NStatus> queue = new Queue<NStatus>();
        public StatusService()
        {
            MessageDistributer.Instance.Subscribe<StatusNotify>(OnStatusNotify);

        }
        public void Dispose()
        {
            MessageDistributer.Instance.Unsubscribe<StatusNotify>(OnStatusNotify);
        }


        public void RegisterEvent(StatusType statusType, StatusNotifyHandler handler)
        {
            if (!eventMap.ContainsKey(statusType))
            {
                eventMap[statusType] = handler;
            }
            else
            {
                eventMap[statusType] += handler;
            }
        }
        private void OnStatusNotify(object sender, StatusNotify message)
        {
            foreach (var NStatus in message.Status)
            {
                //Debug.Log($"status {NStatus.Id} type{NStatus.Type}");
                StatusNotifyHandler handler = null;
                if (eventMap.TryGetValue(NStatus.Type, out handler))
                {
                    handler(NStatus);
                }
                queue.Enqueue(NStatus);
                
            }
            LuaBehaviour.Instance.SafeDoString("ItemGainCtrl:ShowGain()");
        }


    }
}
