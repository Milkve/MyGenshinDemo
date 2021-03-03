using Common;
using GameServer.Entities;
using Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Description;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Managers
{
    class EntityManager : Singleton<EntityManager>
    {
        private int entityId = 0;
        public List<Entity> allEntities = new List<Entity>();
        public Dictionary<int, List<Entity>> mapEntities = new Dictionary<int, List<Entity>>();



        public void Init()
        {

        }


        public void AddEntity(int mapId, Entity entity)
        {
            entity.EntityData.Id = ++entityId;

            allEntities.Add(entity);
            List<Entity> entities = null;
            if (!mapEntities.TryGetValue(mapId, out entities))
            {
                entities = new List<Entity>();
                mapEntities[mapId] = entities;
            }
            mapEntities[mapId].Add(entity);


        }


        public void RemoveEntity(int mapId, Entity entity)
        {
            if (allEntities.Contains(entity)){
            allEntities.Remove(entity);
            }
            if (mapEntities.ContainsKey(mapId)&& mapEntities[mapId].Contains(entity))
            {
                mapEntities[mapId].Remove(entity);
            }
        }
    }
}
