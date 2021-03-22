using System.Runtime.InteropServices;
using UnityEditor;
using UnityEngine;

#if UNITY_EDITOR

#endif

public class TapticEngine : MonoBehaviour
{
    
#if UNITY_IOS
    [DllImport("__Internal")]
    private static extern void _PlayTaptic(string type);
#endif

    public static void TriggerWarning()
    {
#if UNITY_EDITOR
        if (!EditorApplication.isPlaying)
#endif
#if UNITY_IOS
            _PlayTaptic("warning");
#else
            return;
#endif
    }

    public static void TriggerError()
    {
#if UNITY_EDITOR
        if (!EditorApplication.isPlaying)
#endif
#if UNITY_IOS
            _PlayTaptic("error");
#else
            return;
#endif
    }

    public static void TriggerSuccess()
    {
#if UNITY_EDITOR
        if (!EditorApplication.isPlaying)
#endif
#if UNITY_IOS
            _PlayTaptic("success");
#else
            return;
#endif
    }

    public static void TriggerLight()
    {
#if UNITY_EDITOR
        if (!EditorApplication.isPlaying)
#endif
#if UNITY_IOS
            _PlayTaptic("light");
#else
            return;
#endif
    }

    public static void TriggerMedium()
    {
#if UNITY_EDITOR
        if (!EditorApplication.isPlaying)
#endif
#if UNITY_IOS
            _PlayTaptic("medium");
#else
            return;
#endif
    }

    public static void TriggerHeavy()
    {
#if UNITY_EDITOR
        if (!EditorApplication.isPlaying)
#endif
#if UNITY_IOS
            _PlayTaptic("heavy");
#else
            return;
#endif
    }

    public static void TriggerSelectionChange()
    {
#if UNITY_EDITOR
        if (!EditorApplication.isPlaying)
#endif
#if UNITY_IOS
            _PlayTaptic("selectionChange");
#else
            return;
#endif
    }
}