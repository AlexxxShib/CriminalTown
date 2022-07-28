using UnityEngine;

namespace Mobiray.Helpers
{
    public class FollowComp : MonoBehaviour
    {
        public Transform go;

        public Vector3 direction;
        public Vector3 dif;

        public float smoothSpeed = 0.1f;

        private bool _difCalculated;

        private void Awake()
        {
            if (!go) return;
            if (_difCalculated) return;
            
            RecalculateDif();
        }

        public void SetFollowGO(Transform go)
        {
            this.go = go;
            RecalculateDif();
        }

        private void Update()
        {
            if (!_difCalculated) return;
            
            var pos  = transform.position;
            var start = pos;
            var newPos = go.position + dif;

            if (direction.x > 0) pos.x = newPos.x;
            if (direction.y > 0) pos.y = newPos.y;
            if (direction.z > 0) pos.z = newPos.z;

            transform.position = Vector3.Lerp(start, pos, smoothSpeed);

            // transform.position = Vector3.Lerp(pos, newPos, SmoothSpeed);
        }

        public void RecalculateDif(bool ignoreXDif = false)
        {
            dif = transform.position - go.position;
            if (ignoreXDif)
            {
                dif.x = 0;
            }
            _difCalculated = true;
        }
    }
}