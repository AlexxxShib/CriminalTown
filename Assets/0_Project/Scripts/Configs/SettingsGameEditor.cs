using UnityEditor;
using UnityEngine;

namespace Template.Configs
{

#if UNITY_EDITOR
    
    [CustomEditor(typeof(GameSettings))]
    public class SettingsGameEditor : Editor
    {

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            var component = (GameSettings) target;

            if (GUILayout.Button("Clear Saves"))
            {
                component.ClearSaves();
            }
        }

    }
    
#endif

}