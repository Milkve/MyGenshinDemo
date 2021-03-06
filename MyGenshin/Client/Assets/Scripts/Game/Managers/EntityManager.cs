using Common;
using Entities;
using Models;
using SkillBridge.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Managers
{
    class EntityManager : MonoSingleton<EntityManager>
    {
        public interface IEntityNotify
        {
            void OnEntityRemoved();
            void OnEntityChanged(NEntity nEntity);
            void OnEntityEvent(NEntityEvent entityEvent);
        }
        Dictionary<int, Entity> entities = new Dictionary<int, Entity>();

        public Dictionary<int, IEntityNotify> notifies = new Dictionary<int, IEntityNotify>();


        public void RegisterEntityNotify(Entity entity,IEntityNotify entityNotify)
        {
            if (!notifies.ContainsKey(entity.entityId) || notifies[entity.entityId] == null)
            {
                notifies.Add(entity.entityId, entityNotify);
            }
        }
        public void AddEnity(Entity entity)

        {
            if (!entities.ContainsKey(entity.entityId) || entities[entity.entityId] == null)
            {
                entities.Add(entity.entityId, entity);
            }


        }

        public void RemoveEntity(Entity entity)
        {
            if (this.entities.ContainsKey(entity.entityId))
            {
                entities.Remove(entity.entityId);

            }
            if (this.notifies.ContainsKey(entity.entityId))
            {
                notifies[entity.entityId].OnEntityRemoved();
                notifies.Remove(entity.entityId);
            }
        }
        public void OnEnitySync(NEntitySync nEntitySync)
        {
            Entity entity ;
            this.entities.TryGetValue(nEntitySync.Id, out entity);
            if (entities != null)
            {
                if (nEntitySync.Entity != null)
                {
                    entity.SetEntityData(nEntitySync.Entity);

                }
                if (notifies.ContainsKey(nEntitySync.Id))
                {
                    //notifies[nEntitySync.Id].OnEntityChanged(nEntitySync.Entity);
                    notifies[nEntitySync.Id].OnEntityEvent(nEntitySync.Event);
                }

            }

        }
    }
}
