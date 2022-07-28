using System;
using System.Reflection;

#if UNITY_2020
using UnityEditor.Experimental.SceneManagement;
#endif

#if UNITY_2021
using UnityEditor.SceneManagement;
#endif

public class PrefabSavingUtil {

    public static void SavePrefab(PrefabStage prefabStage)
    {
        if (prefabStage == null)
            throw new ArgumentNullException();

        var savePrefabMethod = prefabStage.GetType().GetMethod("SavePrefab", BindingFlags.NonPublic | BindingFlags.Instance);
        if (savePrefabMethod == null)
            throw new InvalidOperationException();

        savePrefabMethod.Invoke(prefabStage, null);
    }
}
