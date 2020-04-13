using Mobiray.Common;
using UnityEngine;

namespace Template.Pool
{

    public class PoolableGameObject : MonoBehaviour
    {

        public EntityType Type;

        public float ReturnTime = -1;

        private void OnEnable()
        {
            if (ReturnTime <= 0) return;
            
            this.StartTimer(ReturnTime, ReturnToPool);
        }

        public void ReturnToPool()
        {
            if (!gameObject.activeSelf) return;
            
            var pool = ToolBox.Get<CommonPool>();
            pool.Return(Type, gameObject);
        }

    }

}