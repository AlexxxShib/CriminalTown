using UnityEngine;

namespace Mobiray.Helpers
{
    public class FixedCameraMoveSmooth : LeanCameraMoveSmooth
    {
        [Space] public MovingType Type;

        public float MinPosition;
        public float MaxPosition;

        [Space] public float MinXPosition;
        public float MaxXPosition;

        private Vector3 startPosition;

        private void Awake()
        {
            startPosition = transform.position;
        }

        protected override void LateUpdate()
        {
            base.LateUpdate();

            var position = transform.position;

            if (Type == MovingType.Z)
            {
//                position.x = startPosition.x;
                position.y = startPosition.y;
                position.z = Mathf.Clamp(position.z, MinPosition, MaxPosition);
            }
            else
            {
                position.x = Mathf.Clamp(position.x, MinPosition, MaxPosition);
                position.y = startPosition.y;
                position.z = startPosition.z;
            }

            position.x = Mathf.Clamp(position.x, MinXPosition, MaxXPosition);

            transform.position = position;
        }
    }
    
    public enum MovingType
    {
        X,
        Z
    }
}