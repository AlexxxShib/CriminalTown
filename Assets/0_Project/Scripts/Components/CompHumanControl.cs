using System;
using Mobiray.Common;
using UnityEngine;
using UnityEngine.AI;

namespace CriminalTown.Entities
{
    public class CompHumanControl : MonoBehaviour
    {

        public BaseJoystick joystick;

        public bool IsNavigable => _agent != null && _agent.enabled && _agent.isOnNavMesh;
        public bool IsMoving => _isMoving;

        public bool inputEnabled = true;

        private NavMeshAgent _agent;
        private Animator _animator;
        
        private static readonly int AnimIdMoving = Animator.StringToHash("isMoving");
        private static readonly int AnimIdCarrying = Animator.StringToHash("carrying");
        private static readonly int AnimIdDance = Animator.StringToHash("dance");
        private static readonly int AnimIdFall = Animator.StringToHash("fall");

        private bool _hasJoystick;
        private bool _isMoving;
        private float _prevSpeed;

        private void Awake()
        {
            _agent = GetComponent<NavMeshAgent>();
            
            _animator = GetComponentInChildren<Animator>();

            _hasJoystick = joystick != null;
        }

        public void SetDance()
        {
            _animator.SetTrigger(AnimIdDance);
        }

        private void Update()
        {
            _isMoving = IsNavigable && _agent.remainingDistance > _agent.stoppingDistance;

            _animator.SetBool(AnimIdMoving, _isMoving);

            if (IsNavigable && _hasJoystick)
            {
                // Debug.Log($"joystick {Joystick.Direction3D}");

                var direction = joystick.Direction3D;
                
                if (!inputEnabled) direction = Vector3.zero;

                var destination = transform.position + direction;

                _agent.SetDestination(destination);
            }
        }
    }
}