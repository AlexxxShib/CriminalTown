using UnityEngine;

namespace MobirayCore.Scripts.Components
{
    public class Rotator : MonoBehaviour
    {
        public Vector3 EulerSpeed = Vector3.zero;

        private void Update()
        {
            transform.Rotate(EulerSpeed * Time.deltaTime);
        }
    }
}