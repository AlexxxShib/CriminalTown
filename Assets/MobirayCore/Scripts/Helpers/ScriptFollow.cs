using UnityEngine;

namespace Mobiray.Helpers
{
    public class ScriptFollow : MonoBehaviour
    {
        public Transform GO;

        public Vector3 Direction;

        public Vector3 dif;

        private void Awake()
        {
            dif = transform.position - GO.position;
        }

        private void FixedUpdate()
        {
            var pos = transform.position;
            var newPos = GO.position + dif;

            if (Direction.x > 0) pos.x = newPos.x;
            if (Direction.y > 0) pos.y = newPos.y;
            if (Direction.z > 0) pos.z = newPos.z;

            transform.position = pos;
        }
    }
}