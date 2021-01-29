using System;
using System.Collections.Generic;
using Mobiray.Common;
using UnityEngine;

namespace Template.Pool
{
    public enum EntityType
    {
        MONEY_FX
    }

    [CreateAssetMenu(fileName = "CommonPool", menuName = "Tools/CommonPool")]
    public class CommonPool : ScriptableObject, INeedInitialization
    {
        public List<PoolPrefab> PoolPrefabs;

        private Mobiray.Helpers.Pool pool;
        private Transform parent;

        private Dictionary<EntityType, GameObject> prefabsMap = new Dictionary<EntityType, GameObject>();

        public void Initialize()
        {
            pool = ToolBox.Get<Mobiray.Helpers.Pool>();
            parent = GameObject.Find("[POOL]").transform;

            foreach (var poolPrefab in PoolPrefabs)
            {
                pool.Instantiate(poolPrefab.Prefab, poolPrefab.Count, parent);

                prefabsMap.Add(poolPrefab.Type, poolPrefab.Prefab);
            }
        }

        public GameObject Create(EntityType type, bool isActive = true)
        {
            var prefab = prefabsMap[type];

            var go = pool.Create(prefab);
            go.transform.parent = parent;
            go.SetActive(isActive);

            return go;
        }

        public void Return(EntityType type, GameObject go)
        {
            var prefab = prefabsMap[type];
            go.transform.parent = parent;
            pool.Return(prefab, go);
        }
    }

    [Serializable]
    public class PoolPrefab
    {
        public EntityType Type;

        public GameObject Prefab;

        public int Count;
    }
}