using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEditor;
using UnityEngine;

namespace MobirayCore.Loading
{
    
#if UNITY_EDITOR
    public class AssetLoader
    {

        public static void Save(GameObject asset, string folder)
        {
            if (asset.transform.childCount == 0)
            {
                Debug.LogError($"empty asset {asset.name}!");
                return;
            }
            
            var path = folder + asset.name + ".prefab";

            PrefabUtility.SaveAsPrefabAsset(asset, path, out var success);
            
            Debug.Log($"save asset {asset.name} to {path} : success {success}");
        }

        public static GameObject Load(Transform root, string folder, string assetName)
        {
            var path = folder + assetName + ".prefab";

            var asset = PrefabUtility.LoadPrefabContents(path);
            
            Debug.Log($"loaded asset {asset.name} from {path}");

            return asset;
        }
    }
    
#endif
    
}