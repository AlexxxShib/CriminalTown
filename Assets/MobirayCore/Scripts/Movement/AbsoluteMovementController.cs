using System;
using Mobiray.Common;
using UnityEngine;

namespace Mobiray.Movement
{
    public enum MoveState
    {
        MOVE,
        STOP
    }

    public class AbsoluteMovementController : MonoBehaviour
    {
        // Start is called before the first frame update
        public BaseJoystick Joystick;

        // public Transform Transform;

        public float MaxSpeed = 1;

        public bool Acceleration = false;

        public float Drag = 1f;

        public bool RotateByX = false;

        public float MaxRotationX = 0;

        public float RotationSpeed = 1;

        public float MovementTolerance = 0.01f;

        public float MoveToStopSmoothingTime = 0.25f;

        public float DefaultY = 0;

        public bool FollowingSoldier = false;

        public bool ShowJoystickValues = false;

        private MoveState moveState = MoveState.STOP;
        public MoveState MoveState => moveState;
        public Action<MoveState,MoveState> OnMoveStateChanged;

        public float speed;
        protected Quaternion lookRotation;

        protected Rigidbody rigidbody;
        protected bool rigidbodyFound;

        private void Start()
        {
            rigidbody = GetComponent<Rigidbody>();
            rigidbodyFound = rigidbody != null;
        }

        private void SetState(MoveState state)
        {
            if (state == moveState) return;

            var lastState = moveState;
            moveState = state;

            if (state == MoveState.STOP && lastState == MoveState.MOVE && gameObject.activeSelf)
            {
                this.StartTimer(MoveToStopSmoothingTime, () =>
                {
                    if (lastState != moveState)
                    {
                        Debug.Log($"{lastState} -> {moveState}");
                        OnMoveStateChanged?.Invoke(lastState, moveState);
                    }
                });
            }
            else
            {
                Debug.Log($"{lastState} -> {moveState}");
                OnMoveStateChanged?.Invoke(lastState, moveState);
            }
        }

        private void OnDisable()
        {
            SetState(MoveState.STOP);
        }

        protected virtual void Update()
        {
            if (rigidbodyFound)
            {
                rigidbody.velocity = Vector3.zero;
                rigidbody.angularVelocity = Vector3.zero;
            }

            var moving = true;

            if (ShowJoystickValues)
            {
                Debug.Log($"joystick {Joystick.Horizontal} x {Joystick.Vertical}");
            }

            var direction = new Vector3(Joystick.Horizontal, 0, Joystick.Vertical);
            
            if (direction.magnitude < MovementTolerance)
            {
                moving = false;
                if (!Acceleration)
                {
                    speed = 0;
                    SetState(MoveState.STOP);
                    return;
                }
            }

            if (Acceleration)
            {
                if (moving && speed < MaxSpeed)
                {
                    speed += Drag * Time.deltaTime;
                }

                if (!moving && speed > 0)
                {
                    speed -= 2 * Drag * Time.deltaTime;

                    if (speed <= 0)
                    {
                        speed = 0;
                        SetState(MoveState.STOP);
                        UpdateRotation();
                        return;
                    }
                }
            }
            else
            {
                speed = FollowingSoldier ? MaxSpeed * direction.magnitude : MaxSpeed;
                if (speed == 0)
                {
                    SetState(MoveState.STOP);
                    return;
                }
            }

            UpdateRotation();

            var cashedTransform = transform;

            SetState(MoveState.MOVE);

            if (direction.magnitude >= MovementTolerance)
            {
                lookRotation = Quaternion.LookRotation(direction);
            }

            transform.rotation = Quaternion.Lerp(transform.rotation, lookRotation,
                // RotationSpeed * (speed / MaxSpeed) * Time.deltaTime);
                RotationSpeed * Time.deltaTime);
            // transform.rotation = lookRotation;

            var angle = Quaternion.Angle(cashedTransform.rotation, lookRotation);

            //((cos x) + 1) / 2
            var angeleSpeedK = (Mathf.Cos(angle * Mathf.Deg2Rad) + 1) / 2;

            var nextPos = cashedTransform.position +
                          transform.forward * (speed * angeleSpeedK * Time.deltaTime);

            // cashedTransform.position = nextPos;

            cashedTransform.position = nextPos.ChangeY(DefaultY);
        }

        private void UpdateRotation()
        {
            if (!RotateByX) return;

            var rotation = transform.rotation.eulerAngles;
            rotation.x = Mathf.Lerp(0, MaxRotationX, speed / MaxSpeed);
            transform.rotation = Quaternion.Euler(rotation);
        }
    }
}