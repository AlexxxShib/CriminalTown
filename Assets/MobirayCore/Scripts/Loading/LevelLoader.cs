using System;
using System.Threading.Tasks;
using UnityEngine;

namespace MobirayCore.Loading
{
    public class LevelLoader : MonoBehaviour
    {

        public string Folder = "Assets/0_Project/Levels/";

        public async Task LoadLevel()
        {
            var resourcePath = $"{Folder}{gameObject.name}.prefab";
            
            Debug.Log($"loading resource started {resourcePath}");

            var startTime = DateTime.Now;

            /*var level = await Addressables
                .InstantiateAsync(resourcePath, transform).Task;
            
            Debug.Log($"loading resource finished {resourcePath} time : {(DateTime.Now - startTime).TotalSeconds}");
            
            level.SetActive(true);*/
        }
    }
}