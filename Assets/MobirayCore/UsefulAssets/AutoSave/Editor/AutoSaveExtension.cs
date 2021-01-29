#if UNITY_EDITOR

using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

namespace EckTechGames
{
	[InitializeOnLoad]
	public class AutoSaveExtension
	{
		// Static constructor that gets called when unity fires up.
		static AutoSaveExtension()
		{
			EditorApplication.playModeStateChanged += AutoSaveWhenPlaymodeStarts;
		}

		private static void AutoSaveWhenPlaymodeStarts(PlayModeStateChange state)
		{
			// If we're about to run the scene...
			if (EditorApplication.isPlayingOrWillChangePlaymode && !EditorApplication.isPlaying)
			{
				// Save the scene and the assets.
				EditorSceneManager.SaveScene(SceneManager.GetActiveScene());
				AssetDatabase.SaveAssets();
			}
		}
	}
}

#endif