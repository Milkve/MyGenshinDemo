using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Managers;
using SkillBridge.Message;
using UnityEngine;

namespace Entities
{
    public class Character : Entity
    {
        public NCharacterInfo Info;

        public Common.Data.CharacterDefine Define;

        public string Name
        {
            get
            {
                if (this.Info.Type == CharacterType.Player)
                    return this.Info.Name;
                else
                    return this.Define.Name;
            }
        }
        
        /// <summary>
        /// 判断此角色是否为当前玩家
        /// </summary>
        public bool IsPlayer
        {
            
            get { return this.Info.Id == Models.User.Instance.CurrentCharacter.Id; }
        }

        public Character(NCharacterInfo info) : base(info.Entity)
        {
            this.Info = info;
            this.Define = DataManager.Instance.Characters[info.Tid];
        }





        public void SetEntityData(Vector3 position,Vector3 direction,float speed)
        {
            this.position = GameObjectTool.WorldToLogic(position);
            this.direction = GameObjectTool.WorldToLogic(direction);
            this.speed = GameObjectTool.WorldToLogic(speed);
        }
    }
}
