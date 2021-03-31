using UnityEngine;

namespace Mobiray.Movement
{
    public class CustomJoystick : BaseJoystick
    {
        public float Sensitivity = 1;

        public bool UpdateDirection(float horizontal, float vertical)
        {
            Horizontal = horizontal;
            Vertical = vertical;

            // Debug.Log($"direction {Direction} with length {Direction.magnitude}");

            if (Direction.magnitude <= Sensitivity)
            {
                Vertical = Horizontal = 0;
                return true;
            }

            return false;
        }

        public bool UpdateDirection(Vector2 direction)
        {
            return UpdateDirection(direction.x, direction.y);
        }

        public bool UpdateDirection(Vector3 directionXZ)
        {
            return UpdateDirection(directionXZ.x, directionXZ.z);
        }
    }
}