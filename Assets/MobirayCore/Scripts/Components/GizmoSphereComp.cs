using UnityEngine;

namespace Mobiray.Helpers
{
    public class GizmoSphereComp : MonoBehaviour
    {
        public float radius = 1;
        public Color color = Color.red;

        private void OnDrawGizmos()
        {
            Gizmos.color = color;
            Gizmos.DrawSphere(transform.position, radius);
        }
    }
}