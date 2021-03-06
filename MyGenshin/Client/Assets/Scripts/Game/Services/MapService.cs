using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common;
using Models;
using Network;
using SkillBridge.Message;
using UnityEngine.Events;
using UnityEngine;
using Common.Data;
using Managers;
using Entities;
using System.Runtime.InteropServices;

namespace Services
{
    class MapService : Singleton<MapService>, IDisposable
    {
        public UnityAction<MapDefine> OnEnterMap;

        public int currentMapId = 0;

        public UnityAction OnLevelMap;

        public MapService()
        {
            
            MessageDistributer.Instance.Subscribe<MapCharacterEnterResponse>(this.OnMapCharacterEnter);
            MessageDistributer.Instance.Subscribe<MapCharacterLeaveResponse>(this.OnMapCharacterLeave);
            MessageDistributer.Instance.Subscribe<MapEntitySyncResponse>(this.OnMapEntitySync);
        }



        public void Dispose()
        {   
            MessageDistributer.Instance.Unsubscribe<MapCharacterEnterResponse>(this.OnMapCharacterEnter);
            MessageDistributer.Instance.Unsubscribe<MapCharacterLeaveResponse>(this.OnMapCharacterLeave);
        }

        private void OnMapCharacterEnter(object sender, MapCharacterEnterResponse response)
        {
            Debug.LogFormat("OnMapCharacterEnter:{0} [{1}]", response.Characters, response.mapId);

            Debug.Log($"CurrentCharacterID:{User.Instance.CurrentCharacter.Id}");
            foreach (NCharacterInfo nCharacter in response.Characters)
            {
                Debug.Log($"nCharacterID:{nCharacter.Id}");
                if (nCharacter.Id == User.Instance.CurrentCharacter.Id)
                {
                    User.Instance.CurrentCharacter = nCharacter;
                }
                CharacterManager.Instance.AddCharacter(nCharacter);
            }
            if (response.mapId != this.currentMapId)
            {
                EnterMap(response.mapId);
                this.currentMapId = response.mapId;
            }


        }

        private void OnMapCharacterLeave(object sender, MapCharacterLeaveResponse response)
        {
            Debug.LogFormat("OnMapCharacterLeave:{0}]", response.characterId);
            if (response.characterId == User.Instance.CurrentCharacter.Entity.Id)
            {
                //if (GobelManager.Instance.isBack2Select)
                //{
                //    Back2Select();
                //}
                if (OnLevelMap != null)
                {
                    OnLevelMap();
                }
                CharacterManager.Instance.Clear();
            }
            else
            {
                CharacterManager.Instance.RemoveCharacter(response.characterId);
            }
        }

        private void Back2Select()
        {
            this.currentMapId = 0;
            User.Instance.Reset();
            //GobelManager.Instance.isBack2Select = false;
            SceneManager.Instance.LoadScene("CharacterSelect");
        }


        private void EnterMap(int mapId)
        {
            Debug.LogFormat("EnterMap: Start Enter Map {0}", mapId);
            if (DataManager.Instance.Maps.ContainsKey(mapId))
            {
                MapDefine map = DataManager.Instance.Maps[mapId];
                User.Instance.CurrentMap = map;
                SceneManager.Instance.LoadScene(map.Resource);

                if (OnEnterMap != null)
                {
                    OnEnterMap(map);
                }
            }
            else
                Debug.LogErrorFormat("EnterMap: Map {0} not existed", mapId);
        }

        public void SendMapEntitySync(NEntityEvent entityEvent, Character character)
        {


            Debug.LogFormat("SendMapEntityData {0}", GameObjectTool.LogicToWorld(character.EntityData.Position));
            NetMessage netMessage = new NetMessage()
            {
                Request = new NetMessageRequest()
                {
                    mapEntitySync = new MapEntitySyncRequest()
                    {
                        entitySync = new NEntitySync()
                        {
                            Id = character.entityId,
                            Entity = character.EntityData,
                            Event = entityEvent

                        }
                    }
                }

            };

            NetClient.Instance.SendMessage(netMessage);

        }

        private void OnMapEntitySync(object sender, MapEntitySyncResponse message)
        {
            foreach (NEntitySync nEntitySync in message.entitySyncs)
            {

                EntityManager.Instance.OnEnitySync(nEntitySync);
            }
        }

        internal void SendMapTeleport(int ID)


        {
            Debug.LogFormat("SendMapteleport {0}", ID);
            NetMessage netMessage = new NetMessage()
            {
                Request = new NetMessageRequest()
                {
                    mapTeleport = new MapTeleportRequest()
                    {
                        teleporterId = ID
                    }
                }
            };

            NetClient.Instance.SendMessage(netMessage);
        }

    }
}
