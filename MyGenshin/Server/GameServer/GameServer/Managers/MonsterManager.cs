using GameServer.Entities;
using GameServer.Managers;
using GameServer.Models;
using SkillBridge.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Managers
{
    class MonsterManager
    {
        private Map map;

        public Dictionary<int, Monster> Monsters = new Dictionary<int, Monster>();


        public void Init(Map map)
        {
            this.map = map;
        }
        internal Monster Create(int spaqnMonId,int spawnLevel, NVector3 position,NVector3 direction)
        {
            Monster monster = new Monster(spaqnMonId, spawnLevel, position, direction);
            EntityManager.Instance.AddEntity(this.map.ID, monster);
            monster.Id = monster.entityId;
            monster.Info.EntityId = monster.entityId;
            monster.Info.mapId = this.map.ID;
            Monsters[monster.Id] = monster;
            //TODO
            //this.map.MonsterEnter(monster);
            return monster;

        }
    }
}
