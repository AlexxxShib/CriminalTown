using System;
using Mobiray.Common;
using UnityEngine;
using UnityEngine.AI;

namespace CriminalTown.Entities
{
    public class CompHumanControl : MonoBehaviour
    {

        public BaseJoystick Joystick;

        public bool IsNavigable => agent != null && agent.enabled && agent.isOnNavMesh;

        public bool InputEnabled = true;

        private NavMeshAgent agent;

        private Animator animator;
        
        private static readonly int AnimIdMoving = Animator.StringToHash("isMoving");
        private static readonly int AnimIdCarrying = Animator.StringToHash("carrying");
        private static readonly int AnimIdDance = Animator.StringToHash("dance");
        private static readonly int AnimIdFall = Animator.StringToHash("fall");

        private bool hasJoystick;
        private float prevSpeed;

        private void Awake()
        {
            agent = GetComponent<NavMeshAgent>();
            
            animator = GetComponent<Animator>();

            hasJoystick = Joystick != null;
        }

        public void SetDance()
        {
            animator.SetTrigger(AnimIdDance);
        }

        private void Update()
        {
            var isMoving = false;

            if (IsNavigable)
            {
                isMoving = agent.remainingDistance > agent.stoppingDistance;
            }

            animator.SetBool(AnimIdMoving, isMoving);

            if (IsNavigable && hasJoystick)
            {
                // Debug.Log($"joystick {Joystick.Direction3D}");

                var direction = Joystick.Direction3D;
                
                if (!InputEnabled) direction = Vector3.zero;

                var destination = transform.position + direction;

                agent.SetDestination(destination);
            }
        }
    }
}