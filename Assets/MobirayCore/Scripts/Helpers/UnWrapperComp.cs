using UnityEngine;

namespace Mobiray.Helpers
{
    public class UnWrapperComp : MonoBehaviour
    {
        private Transform _newParent;

        private void Awake()
        {
            _newParent = new GameObject("new_parent").transform;
            _newParent.parent = transform.parent;

            SetNewParent(transform);
        }

        private void SetNewParent(Transform transform)
        {
            foreach (Transform t in transform)
            {
                if (t.childCount == 0)
                {
                    t.parent = _newParent;
                }
                else
                {
                    SetNewParent(t);
                }
            }
        }
    }
}