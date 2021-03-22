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
        public bool IsDebugGameState = false;
        public bool IsSaveGame = true;
        public string PathSaves = "/saves";

        [Space]
        public int MaxSecondsOffline;

        [Space]
        public int TargetFrameRate = 60;

        // public bool IsCustomSave;
        // public GameStateCustomSave CustomSave;

        public void ClearSaves()
        {
            File.Delete(ToolSaver.PathFor(PathSaves, typeof(DataGameState)));

            Debug.Log("DONE!");
        }
    }
}