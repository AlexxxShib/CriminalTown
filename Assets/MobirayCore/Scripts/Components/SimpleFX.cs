using System;
using Mobiray.Common;
using Mobiray.Helpers;
using UnityEngine;

namespace MobirayCore.Components
{
    public class SimpleFX : MonoBehaviour
    {
        public float LifeTime = 2;

        public void Show(Vector3 pos, PoolHelper<SimpleFX> pool)
        {
            transform.position = pos;
            
            if (LifeTime > 0)
            {
                this.StartTimer(LifeTime, () =>
                {
                    pool.Return(this);
                });
            }
        }
    }
}