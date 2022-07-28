using Mobiray.Common;
using NaughtyAttributes;
using UnityEngine;

namespace Mobiray.Helpers
{

#if UNITY_EDITOR
    
    [ExecuteInEditMode]
    public class ChildrenRenamer : MonoBehaviour
    {

        [Space]
        public string Prefix;
        public int StartIndex;

        [Button("Rename")]
        private void Rename()
        {
            var children = transform.GetChildren();

            for (var i = 0; i < children.Count; i++)
            {
                children[i].gameObject.name = $"{Prefix}{i + StartIndex}";
            }
        }

    }
    
#endif

}