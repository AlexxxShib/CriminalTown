using System.IO;
using CriminalTown.Data;
using Mobiray.Common;
using NaughtyAttributes;
using UnityEditor;
using UnityEngine;

namespace CriminalTown.Configs
{
    [ExecuteInEditMode]
    [CreateAssetMenu(fileName = "GameSettings", menuName = "Configs/GameSettings")]
    public class GameSettings : ScriptableObject
    {
        [Header("Saves")]
        public bool isDebugGameState = false;
        public bool isSaveGame = true;
        public string pathSaves = "/saves";

        [Header("Ads")]
        public int interstitialCoolDown = 30;
        public bool adsFreeBuild;
        
        [Header("Other")]
        public int targetFrameRate = 60;
        
        [Button("Open Saves Folder", space:20)]
        public void OpenSavesFolder()
        {
            EditorUtility.RevealInFinder(ToolSaver.PathFor(pathSaves, typeof(DataGameState)));
        }
        
        [Button("Clear Saves & Prefs")]
        public void ClearSaves()
        {
            File.Delete(ToolSaver.PathFor(pathSaves, typeof(DataGameState)));
            PlayerPrefs.DeleteAll();

            Debug.Log("Saves and preferences are cleared...");
        }
    }
}