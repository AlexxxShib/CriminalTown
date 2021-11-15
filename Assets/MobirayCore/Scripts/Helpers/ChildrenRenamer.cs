using Mobiray.Common;
using UnityEngine;

namespace Mobiray.Helpers
{

#if UNITY_EDITOR
    
    [ExecuteInEditMode]
    public class ChildrenRenamer : MonoBehaviour
    {

        public bool ButtonRename;

        [Space]
        public string Prefix;
        public int StartIndex;

        private void Update()
        {
            if (ButtonRename)
            {
                ButtonRename = false;
                
                Rename();
            }
        }

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