using UnityEngine;

namespace Mobiray.Helpers
{
    public class Unwrapper : MonoBehaviour
    {
        private Transform newParent;

        private void Awake()
        {
            newParent = new GameObject("new_parent").transform;
            newParent.parent = transform.parent;

            SetNewParent(transform);
        }

        private void SetNewParent(Transform transform)
        {
            foreach (Transform t in transform)
            {
                if (t.childCount == 0)
                {
                    t.parent = newParent;
                }
                else
                {
                    SetNewParent(t);
                }
            }
        }
    }
}