using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

namespace Mobiray.Helpers
{
    
    [Serializable]
    public class PoolHelper<T> where T : MonoBehaviour
    {
        protected List<string> EntityTypes => entitiesPool.Keys.ToList();
        
        public Transform PoolParent;
        
        private Dictionary<string, List<T>> entitiesPool = new Dictionary<string, List<T>>();
        
        public void Init(int entitiesCount = 15)
        {
            for (int i = 0; i < PoolParent.childCount; i++)
            {
                var entity = PoolParent.GetChild(i).GetComponent<T>();
                
                entity.gameObject.SetActive(false);

                entitiesPool.Add(entity.name, new List<T> {entity});
            }

            foreach (var pair in entitiesPool)
            {
                var original = pair.Value[0];
                
                while (pair.Value.Count < entitiesCount)
                {
                    pair.Value.Add(Object.Instantiate(original, PoolParent));
                }
            }
        }

        public T Instantiate(bool enable = true)
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
                entities.Add(Object.Instantiate(entities[0], PoolParent));
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
            entity.transform.parent = PoolParent;

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