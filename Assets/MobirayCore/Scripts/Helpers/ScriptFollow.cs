using UnityEngine;

namespace Mobiray.Helpers
{
    public class ScriptFollow : MonoBehaviour
    {
        public Transform GO;

        public Vector3 Direction;

        public Vector3 dif;

        public float SmoothSpeed = 0.1f;

        private bool difCalculated;

        private void Awake()
        {
            if (!GO) return;
            if (difCalculated) return;
            
            RecalculateDif();
        }

        public void SetFollowGO(Transform go)
        {
            GO = go;
            RecalculateDif();
        }

        private void Update()
        {
            if (!difCalculated) return;
            
            var pos  = transform.position;
            var start = pos;
            var newPos = GO.position + dif;

            if (Direction.x > 0) pos.x = newPos.x;
            if (Direction.y > 0) pos.y = newPos.y;
            if (Direction.z > 0) pos.z = newPos.z;

            transform.position = Vector3.Lerp(start, pos, SmoothSpeed);

            // transform.position = Vector3.Lerp(pos, newPos, SmoothSpeed);
        }

        public void RecalculateDif(bool ignoreXDif = false)
        {
            dif = transform.position - GO.position;
            if (ignoreXDif)
            {
                dif.x = 0;
            }
            difCalculated = true;
        }
    }
}