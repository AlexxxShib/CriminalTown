using System.IO;
using Template.Data;
using Mobiray.Common;
using UnityEngine;

namespace Template.Configs
{
    [ExecuteInEditMode]
    [CreateAssetMenu(fileName = "GameSettings", menuName = "Configs/GameSettings")]
    public class GameSettings : ScriptableObject
    {
        [Header("Saves")]
        public bool IsDebugGameState = false;
        public bool IsSaveGame = true;
        public string PathSaves = "/saves";

        [Header("Ads")]
        public int InterstitialCoolDown = 30;
        public bool AdsFreeBuild;
        
        [Header("Other")]
        public int TargetFrameRate = 60;
        
        public void ClearSaves()
        {
            File.Delete(ToolSaver.PathFor(PathSaves, typeof(DataGameState)));
            PlayerPrefs.DeleteAll();

            Debug.Log("Saves and preferences are cleared...");
        }
    }
}