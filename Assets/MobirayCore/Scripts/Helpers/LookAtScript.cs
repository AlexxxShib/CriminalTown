using System;
using UnityEngine;

namespace Mobiray.Helpers
{
    public class LookAtScript : MonoBehaviour
    {
        private bool knowPoint;
        private Transform followPoint;

        public void SetPoint(Transform point)
        {
            followPoint = point;
            knowPoint = true;
        }

        private void FixedUpdate()
        {
            if (!knowPoint) return;
            
            transform.LookAt(followPoint);
        }
    }
}