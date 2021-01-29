using System.Collections.Generic;
using UnityEngine;

namespace Mobiray.Helpers
{
    [CreateAssetMenu(fileName = "Pool", menuName = "Tools/Pool")]
    public class Pool : ScriptableObject
    {
        private Dictionary<GameObject, List<GameObject>> items = new Dictionary<GameObject, List<GameObject>>();

        public void Instantiate(GameObject prefab, int count) => Instantiate(prefab, count, null);

        public void Instantiate(GameObject prefab, int count, Transform parent)
        {
            var list = items[prefab] = new List<GameObject>(count);

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

        public T Create<T>(GameObject original) => Create(original).GetComponent<T>();

        public GameObject Create(GameObject original)
        {
            var pool = items[original];

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
            items[original].Add(go);
        }

        public void Clear() => items.Clear(); 
    }
}