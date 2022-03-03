using System.IO;
using Template.Data;
using Mobiray.Common;
using NaughtyAttributes;
using UnityEngine;

namespace Template.Configs
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
        
        [Button("Clear Saves & Prefs", space:20)]
        public void ClearSaves()
        {
            File.Delete(ToolSaver.PathFor(pathSaves, typeof(DataGameState)));
            PlayerPrefs.DeleteAll();

            Debug.Log("Saves and preferences are cleared...");
        }
    }
}