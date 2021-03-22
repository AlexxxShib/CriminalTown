using System;
using UnityEngine;

namespace Mobiray.Helpers
{
    public class CameraFollowScript : MonoBehaviour
    {

        public Transform LookAtPoint;
        public Transform FollowPoint;

        [Space]
        public float SmoothTime = 1;
        
        private Vector3 currentVelocity;
        
        private void FixedUpdate()
        {
            transform.LookAt(LookAtPoint);
            transform.position = Vector3.SmoothDamp(transform.position, 
                FollowPoint.position, ref currentVelocity, SmoothTime);
        }
    }
}