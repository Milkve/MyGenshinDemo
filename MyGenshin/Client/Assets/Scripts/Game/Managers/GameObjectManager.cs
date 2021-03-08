using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Entities;
using Services;
using SkillBridge.Message;
using Models;
using System;

namespace Managers
{
    class GameObjectManager : MonoSingleton<GameObjectManager>
    {

        Dictionary<int, UnityEngine.GameObject> GameObects = new Dictionary<int, UnityEngine.GameObject>();

        protected override void OnStart()
        {
            StartCoroutine(CreateCharacters());
            CharacterManager.Instance.OnCharacterEnter = this.OnCharacterEnter;
            CharacterManager.Instance.OnCharacterLeave = this.OnCharacterLeave;
        }

        private void OnCharacterLeave(int entityId)
        {
            Debug.LogFormat("OnCharacterLeave:characterId{0}", entityId);
            if (GameObects.ContainsKey(entityId))
            {
                DestroyCharacter(GameObects[entityId]);
                GameObects.Remove(entityId);
            }
        }
        private void DestroyCharacter(GameObject character)
        {
            Debug.LogFormat("DestoryCharacter:GameObject {0}", character.name);
            //UIWorldElementsManager.Instance.RemoveCharacter(character.transform);
            Destroy(character);
        }

        private void OnCharacterEnter(Character character)
        {
            CreateCharacter(character);
        }

        IEnumerator CreateCharacters()
        {
            foreach (Character character in CharacterManager.Instance.Characters.Values)
            {
                CreateCharacter(character);
                yield return null;
            }


        }

        public void CreateCharacter(Character character)
        {
            if (!GameObects.ContainsKey(character.entityId) || GameObects[character.entityId] == null)
            {
                UnityEngine.Object obj = ResMgr.GetPrefab(character.Define.Name, character.Define.Resource);

                if (obj == null)
                {
                    Debug.LogErrorFormat("Character[{0}] Resource[{1}] not existed.", character.Define.TID, character.Define.Resource);
                    return;
                }
                GameObject go = (GameObject)Instantiate(obj, this.transform);
                go.name = "Character_" + character.Info.Id + "_" + character.Info.Name;
                GameObects[character.entityId] = go;
                //UIWorldElementsManager.Instance.AddCharacter(go.transform, character);

                InitGameObject(character, go);

            }

        }

        private void InitGameObject(Character character, GameObject go)
        {


            go.transform.position = GameObjectTool.LogicToWorld(character.position);
            go.transform.forward = GameObjectTool.LogicToWorld(character.direction);
            EntityController ec = go.GetComponent<EntityController>();
            if (ec != null)
            {
                ec.isCurrentPlayer = character.IsCurrentPlayer;
                ec.entity = character;
                ec.enabled = true;
            }

            PlayerController pc = go.GetComponent<PlayerController>();
            if (character.IsCurrentPlayer && pc != null)
            {

                User.Instance.CurrentCharacterObject = go;
                pc.ec = ec;
                pc.character = character;
                GameObject playModule = ResMgr.GetPrefab("playModule", "module/common/PlayModule.prefab");
                playModule = Instantiate(playModule);
                DontDestroyOnLoad(playModule);
                Transform player = go.transform;
                playModule.transform.position = player.position;
                playModule.transform.rotation = player.rotation;
                player.SetParent(playModule.transform);
                player.localPosition = Vector3.zero;
                MainPlayerCamera.Instance.SetCurrentPlayer(player, playModule, playModule.GetComponent<CharacterController>());
            }

        }




    }
}
