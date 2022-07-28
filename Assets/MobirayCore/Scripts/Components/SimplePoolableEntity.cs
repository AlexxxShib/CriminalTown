using System;
using Mobiray.Common;
using Mobiray.Helpers;
using UnityEngine;

namespace MobirayCore.Components
{
    public class SimplePoolableEntity : MonoBehaviour
    {
        public float lifeTime = 2;

        public void Show(Vector3 pos, PoolHelper<SimplePoolableEntity> pool)
        {
            transform.position = pos;
            
            if (lifeTime > 0)
            {
                this.StartTimer(lifeTime, () =>
                {
                    pool.Return(this);
                });
            }
        }
    }
}