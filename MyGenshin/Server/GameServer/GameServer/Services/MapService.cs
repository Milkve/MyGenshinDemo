using Common;
using Common.Data;
using GameServer.Entities;
using GameServer.Managers;
using Network;
using SkillBridge.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Services
{
    class MapService : Singleton<MapService>, IDisposable
    {
        public void Dispose()
        {
        }

        public void Init()
        {
            MapManager.Instance.Init();

            MessageDistributer<NetConnection<NetSession>>.Instance.Subscribe<MapEntitySyncRequest>(this.OnEntitySync);
            MessageDistributer<NetConnection<NetSession>>.Instance.Subscribe<MapTeleportRequest>(this.OnMapTeleport);
        }

        private void OnMapTeleport(NetConnection<NetSession> sender, MapTeleportRequest message)
        {
            if (!DataManager.Instance.Teleporters.ContainsKey(message.teleporterId))
            {
                Log.WarningFormat("character {0} request teleport {1} not exist", sender.Session.Character.Info.Id, message.teleporterId);
                return;
            }
            TeleporterDefine sourse= DataManager.Instance.Teleporters[message.teleporterId];
            if (sourse.LinkTo==0 ||!DataManager.Instance.Teleporters.ContainsKey(sourse.LinkTo))
            {
                Log.WarningFormat("character {0} request teleport linkto {1} not exist", sender.Session.Character.Info.Id, message.teleporterId);
                return;
            }
            TeleporterDefine target = DataManager.Instance.Teleporters[sourse.LinkTo];

            MapManager.Instance[sourse.MapID].CharacterLeave( sender.Session.Character);
            sender.Session.Character.Info.mapId = target.MapID;
            sender.Session.Character.Position = target.Position;
            sender.Session.Character.Direction = target.Direction;
            MapManager.Instance[target.MapID].CharacterEnter(sender, sender.Session.Character);


        }

        private void OnEntitySync(NetConnection<NetSession> sender, MapEntitySyncRequest request)
        {
            //Log.InfoFormat("OnEntitySync {0}", request.entitySync.Event.ToString());
            Character character = sender.Session.Character;
            if (character != null)
            {

            MapManager.Instance[character.Data.MapID].EntitySync(request.entitySync);
            }
            
        }

       
    }
}
