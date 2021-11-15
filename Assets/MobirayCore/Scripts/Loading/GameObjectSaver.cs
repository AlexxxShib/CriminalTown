using System;
using Mobiray.Common;
using UnityEngine;

namespace MobirayCore.Loading
{
    
#if UNITY_EDITOR
    
    [ExecuteInEditMode]
    public class GameObjectSaver : MonoBehaviour
    {

        public string Folder = "Assets/0_Project/Levels/";

        [Space]
        public bool ButtonSave;

        public void SaveChildren()
        {
            var children = transform.GetChildren();

            foreach (var child in children)
            {
                AssetLoader.Save(child.gameObject, Folder);

                var assetName = child.gameObject.name;
                
                DestroyImmediate(child.gameObject);
                
                var loader = new GameObject(assetName);
                loader.AddComponent<LevelLoader>().Folder = Folder;
                loader.transform.parent = transform;
                loader.SetActive(false);
            }
        }

        private void Update()
        {
            if (ButtonSave)
            {
                ButtonSave = false;
                SaveChildren();
            }
        }
    }
    
#endif
    
}