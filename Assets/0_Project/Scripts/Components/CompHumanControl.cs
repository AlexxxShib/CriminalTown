using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AI;

namespace CriminalTown.Entities
{
    public class CompHumanControl : MonoBehaviour
    {

        public BaseJoystick joystick;

        public bool IsNavigable => _agent != null && _agent.enabled && _agent.isOnNavMesh;
        public bool IsMoving => _isMoving;

        public float MaxSpeed
        {
            get => _agent.speed;
            set => _agent.speed = value;
        }

        private bool _inputEnabled = true;

        public bool InputEnabled
        {
            get => _inputEnabled;
            set
            {
                _inputEnabled = value;

                if (!_inputEnabled && IsNavigable)
                {
                    _agent.ResetPath();
                }
            }
        }

        private NavMeshAgent _agent;
        private Animator _animator;
        
        private static readonly int AnimIdMoving = Animator.StringToHash("isMoving");
        private static readonly int AnimIdSpeed = Animator.StringToHash("speed");
        private static readonly int AnimIdCarrying = Animator.StringToHash("carrying");
        private static readonly int AnimIdDance = Animator.StringToHash("dance");
        private static readonly int AnimIdFall = Animator.StringToHash("fall");

        private bool _hasJoystick;
        
        private bool _isMoving;

        private Transform _destination;
        private Action<CompHumanControl> _onDestinationFinish;
        private bool _hasDestination;

        private void Awake()
        {
            _agent = GetComponent<NavMeshAgent>();
            _animator = GetComponentInChildren<Animator>();

            _hasJoystick = joystick != null;
        }

        public async void SetPosition(Vector3 pos)
        {
            /*_agent.enabled = false;

            transform.position = pos;
            transform.rotation = rot;

            await Task.Delay(TimeSpan.FromSeconds(Time.deltaTime));

            _agent.enabled = true;*/

            _agent.Warp(pos);
        }

        public void SetDestination(Transform destination, Action<CompHumanControl> listener)
        {
            _destination = destination;
            _onDestinationFinish = listener;
            
            _hasDestination = true;
        }

        public void SetDance()
        {
            _animator.SetTrigger(AnimIdDance);
        }

        private void Update()
        {
            var isNavigable = IsNavigable;
            
            _isMoving = isNavigable && _agent.remainingDistance > _agent.stoppingDistance;

            _animator.SetBool(AnimIdMoving, _isMoving);
            _animator.SetFloat(AnimIdSpeed, isNavigable ? _agent.velocity.magnitude : 0);

            if (!isNavigable)
            {
                return;
            }
            
            if (_hasJoystick)
            {
                var direction = InputEnabled ? joystick.Direction3D : Vector3.zero;
                var destination = transform.position + direction;

                _agent.SetDestination(destination);
                return;
            }

            if (_hasDestination && InputEnabled)
            {
                _agent.SetDestination(_destination.position);

                var finishDistance = (_destination.position - transform.position).magnitude;

                if (finishDistance <= 1f)
                {
                    _onDestinationFinish.Invoke(this);
                    _hasDestination = false;
                }
            }
        }
    }
}