using UnityEngine;

namespace MobirayCore.Scripts.Components
{
    public class RotatorComp : MonoBehaviour
    {
        public Vector3 eulerSpeed = Vector3.zero;

        private void Update()
        {
            transform.Rotate(eulerSpeed * Time.deltaTime);
        }
    }
}