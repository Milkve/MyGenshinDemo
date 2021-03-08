﻿using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SkillBridge.Message;
using GameServer.Entities;

namespace GameServer.Managers
{
    class CharacterManager : Singleton<CharacterManager>
    {
        public Dictionary<int, Character> Characters = new Dictionary<int, Character>();

        public CharacterManager()
        {
        }

        public void Dispose()
        {
        }

        public void Init()
        {

        }

        public void Clear()
        {
            this.Characters.Clear();
        }

        public Character AddCharacter(TCharacter cha)
        {
            Character character = new Character(CharacterType.Player, cha);
            EntityManager.Instance.AddEntity(character.Data.MapID, character);
            this.Characters[cha.ID] = character;
            return character;
        }

        public bool GetCharacter(int characterID,out Character character)
        {
           return Characters.TryGetValue(characterID, out character);
           
        }


        public void RemoveCharacter(int characterId)
        {

            Character character = null;
            if (Characters.TryGetValue(characterId,out character))
            {        
                EntityManager.Instance.RemoveEntity(character.Data.MapID, character);
                this.Characters.Remove(characterId);
                character.Clear();
            }

        }
    }
}
