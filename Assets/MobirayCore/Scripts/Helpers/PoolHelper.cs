using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Mobiray.Helpers
{
    public class PoolHelper<T> where T : MonoBehaviour
    {
        public List<string> EntityTypes => entitiesPool.Keys.ToList();
        
        private Transform poolParent;
        
        private Dictionary<string, List<T>> entitiesPool = new Dictionary<string, List<T>>();
        
        public PoolHelper(Transform poolParent, int entitiesCount = 15)
        {
            this.poolParent = poolParent;
            
            for (int i = 0; i < poolParent.childCount; i++)
            {
                var entity = poolParent.GetChild(i).GetComponent<T>();
                
                entity.gameObject.SetActive(false);

                entitiesPool.Add(entity.name, new List<T> {entity});
            }

            foreach (var pair in entitiesPool)
            {
                var original = pair.Value[0];
                
                while (pair.Value.Count < entitiesCount)
                {
                    pair.Value.Add(Object.Instantiate(original, poolParent));
                }
            }
        }

        public T InstantiateRandom(bool enable = true)
        {
            var keys = EntityTypes;
            var type = keys[Random.Range(0, keys.Count)];

            return Instantiate(type, enable);
        }
        
        public T Instantiate(string type, bool enable = true)
        {
            var entities = entitiesPool[type];

            if (entities.Count == 1)
            {
                entities.Add(Object.Instantiate(entities[0], poolParent));
            }

            var lastIndex = entities.Count - 1;
            var lastEntity = entities[lastIndex];
            entities.RemoveAt(lastIndex);
            
            lastEntity.gameObject.SetActive(enable);
            return lastEntity;
        }

        public void Return(T entity)
        {
            entity.gameObject.SetActive(false);

            var entityName = entity.gameObject.name;
            var type = string.Empty;

            foreach (var entityType in EntityTypes)
            {
                if (entityName.StartsWith(entityType))
                {
                    type = entityType;
                    break;
                }
            }

            if (string.IsNullOrEmpty(type))
            {
                Debug.LogError($"Not found type for {entity}");
                return;
            }
            
            entitiesPool[type].Add(entity);
        }
        
    }
}