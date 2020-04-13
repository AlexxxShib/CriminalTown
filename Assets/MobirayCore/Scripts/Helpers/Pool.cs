using System.Collections.Generic;
using UnityEngine;

namespace Mobiray.Helpers
{
    [CreateAssetMenu(fileName = "Pool", menuName = "Tools/Pool")]
    public class Pool : ScriptableObject
    {
        private Dictionary<int, List<GameObject>> items = new Dictionary<int, List<GameObject>>();

        public void Instantiate(GameObject prefab, int count)
        {
            Instantiate(prefab, count, null);
        }

        public void Instantiate(GameObject prefab, int count, Transform parent)
        {
            var list = items[prefab.GetInstanceID()] = new List<GameObject>(count);

            for (int i = 0; i < count; i++)
            {
                var instance = Instantiate(prefab);
                instance.SetActive(false);

                if (parent != null)
                {
                    instance.transform.parent = parent;
                }

                list.Add(instance);
            }
        }

        public T Create<T>(GameObject original)
        {
            var pool = items[original.GetInstanceID()];

            if (pool.Count == 0)
            {
                return Instantiate(original).GetComponent<T>();
            }

            var first = pool[0];
            pool.RemoveAt(0);

            first.SetActive(true);
            return first.GetComponent<T>();
        }

        public GameObject Create(GameObject original)
        {
            var pool = items[original.GetInstanceID()];

            if (pool.Count == 0)
            {
                return Instantiate(original);
            }

            var first = pool[0];
            pool.RemoveAt(0);

            first.SetActive(true);
            return first;
        }

        public void Return(GameObject original, GameObject go)
        {
            go.SetActive(false);
            items[original.GetInstanceID()].Add(go);
        }
    }

    [System.Serializable]
    public class PoolItem
    {
        public GameObject Prefab;
        public int Count;
    }
}