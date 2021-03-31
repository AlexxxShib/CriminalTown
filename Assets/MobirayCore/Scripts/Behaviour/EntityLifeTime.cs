using System;
using Mobiray.Common;
using UnityEngine;

namespace Mobiray.Behaviour
{
    public class EntityLifeTime : MonoBehaviour
    {
        public float LifeTime = 2f;

        public Action OnTimeOut;

        private void OnEnable()
        {
            this.StartTimer(LifeTime, () =>
            {
                OnTimeOut?.Invoke();
                gameObject.SetActive(false);
            });
        }
    }
}