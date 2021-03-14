using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SkillBridge.Message;

using Common;
using Common.Data;

using Network;
using GameServer.Managers;
using GameServer.Entities;
using System.ServiceModel.Channels;
using GameServer.Services;

namespace GameServer.Models
{
    class Map
    {
        internal class MapCharacter
        {
            public NetConnection<NetSession> connection;
            public Character character;

            public MapCharacter(NetConnection<NetSession> conn, Character cha)
            {
                this.connection = conn;
                this.character = cha;
            }
        }



        public int ID
        {
            get { return this.Define.ID; }
        }
        internal MapDefine Define;

        Dictionary<int, MapCharacter> MapCharacters = new Dictionary<int, MapCharacter>();


        internal Map(MapDefine define)
        {
            this.Define = define;
        }

        internal void Update()
        {
        }



        #region 角色进入
        /// <summary>
        /// 角色进入地图
        /// </summary>
        /// <param name="character"></param>
        internal void CharacterEnter(NetConnection<NetSession> conn, Character character)
        {
            Log.InfoFormat("CharacterEnter: Map:{0} characterId:{1}", this.Define.ID, character.Id);

            character.Info.mapId = this.ID;

            conn.Session.Response.mapCharacterEnter = new MapCharacterEnterResponse();
            conn.Session.Response.mapCharacterEnter.mapId = this.Define.ID;
            conn.Session.Response.mapCharacterEnter.Characters.Add(character.Info);

            foreach (var kv in this.MapCharacters)
            {
                conn.Session.Response.mapCharacterEnter.Characters.Add(kv.Value.character.Info);
                SendCharacterEnterMap(kv.Value.connection, character.Info);
            }

            this.MapCharacters[character.Id] = new MapCharacter(conn, character);

            conn.SendResponse();
        }
        void SendCharacterEnterMap(NetConnection<NetSession> conn, NCharacterInfo character)
        {
            conn.Session.Response.mapCharacterEnter = new MapCharacterEnterResponse();
            conn.Session.Response.mapCharacterEnter.mapId = this.Define.ID;
            conn.Session.Response.mapCharacterEnter.Characters.Add(character);

            conn.SendResponse();
        }

        #endregion


        #region 角色离开
        public void CharacterLeave(Character character)

        {
            if (character == null) return;
            Log.InfoFormat("CharacterLeave: Map:{0} characterId:{1}", this.Define.ID, character.Id);

            foreach (var kv in this.MapCharacters)
            {
                SendCharacterLeaveMap(kv.Value.connection, character.entityId);
            }
            this.MapCharacters.Remove(character.Id);
        }

        void SendCharacterLeaveMap(NetConnection<NetSession> conn, int entityId)
        {
            Log.InfoFormat("SendCharacterLeave:characterId:{0}", entityId);

            conn.Session.Response.mapCharacterLeave = new MapCharacterLeaveResponse()
            {
                entityId = entityId
            };

            conn.SendResponse();
        }


        #endregion


        #region 角色同步

        public void EntitySync(NEntitySync nEntitySync)
        {
            foreach (var kv in this.MapCharacters)
            {
                if (kv.Value.character.entityId == nEntitySync.Id)
                {
                    kv.Value.character.SetEntityData(nEntitySync.Entity);

                }
                else
                {
                    SendEntitySync(kv.Value.connection, nEntitySync);
                }
            }
        }

        void SendEntitySync(NetConnection<NetSession> conn, NEntitySync nEntitySync)
        {
            //Log.InfoFormat("SendEntitySync:nEntityEvent:{0}", nEntitySync.Event.ToString());
            conn.Session.Response.mapEntitySync = new MapEntitySyncResponse();
            conn.Session.Response.mapEntitySync.entitySyncs.Add(nEntitySync);
            conn.SendResponse();
        }


        #endregion

    }
}
