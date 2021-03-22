using UnityEngine;

namespace Mobiray.Helpers
{
    public class GizmoSphere : MonoBehaviour
    {
        public float Radius = 1;
        public Color Color = Color.red;

        private void OnDrawGizmos()
        {
            Gizmos.color = Color;
            Gizmos.DrawSphere(transform.position, Radius);
        }
    }
}