using Entities;
using SkillBridge.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.Events;

namespace Managers
{
    class CharacterManager : Singleton<CharacterManager>, IDisposable
        
        
    {
        
        public UnityAction<Character> OnCharacterEnter;
        public UnityAction<int> OnCharacterLeave;
        public Dictionary<int, Character> Characters = new Dictionary<int, Character>();


        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public void AddCharacter(NCharacterInfo nCharacterInfo)
        {
            Debug.Log($"CharacterManager.AddCharacter ID:{ nCharacterInfo.Id}");
            Character character = new Character(nCharacterInfo);
            this.Characters.Add(nCharacterInfo.EntityId, character);

            if (OnCharacterEnter != null)
            {
                OnCharacterEnter(character);
            }
        }

        internal void RemoveCharacter(int entityID)
        {
           if (this.Characters.ContainsKey(entityID)){
                this.Characters.Remove(entityID);
                if (OnCharacterLeave != null)
                {
                    OnCharacterLeave(entityID);
                }
            }
        }

        internal void Clear()
        {
            int[] keys = this.Characters.Keys.ToArray();
            foreach (int key in keys)
            {
                this.RemoveCharacter(key);
            }
            this.Characters.Clear();
        }
    }
}
