using System;
using UnityEngine;

namespace Mobiray.Movement
{
    
    [RequireComponent(typeof(AbsoluteMovementController))]
    [RequireComponent(typeof(CustomJoystick))]
    public class FollowController : MonoBehaviour
    {
        public Transform FollowObject;
        public bool KnowFollowObject;


        private bool initlized;

        private AbsoluteMovementController movementController;
        protected CustomJoystick customJoystick;

        public Action ManOnPoint;

        public void SetFollowObject(Transform followObject)
        {
            FollowObject = followObject;
            KnowFollowObject = true;

            Init();
            movementController.Joystick = customJoystick;
        }

        public void ForgetFollowObject()
        {
            FollowObject = null;
            KnowFollowObject = false;
        }

        public virtual void Init()
        {
            if (initlized) return;
            
            movementController = GetComponent<AbsoluteMovementController>();
            customJoystick = GetComponent<CustomJoystick>();
            
            movementController.Joystick = customJoystick;

            initlized = true;
        }

        private void Awake()
        {
            Init();
        }

        private void Update()
        {
            if (!KnowFollowObject) return;

            var direction = FollowObject.transform.position - transform.position;
            // direction = direction.normalized;

            if (customJoystick.UpdateDirection(direction.x, direction.z))
            {
                ManOnPoint?.Invoke();
                ManOnPoint = null;
            }
        }
    }
}