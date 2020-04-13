using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace Mobiray.Helpers
{
    [AttributeUsage(AttributeTargets.Field)]
    public class PreBuildCheck : Attribute
    {
        public PreBuildCheck(bool defaultBoolValue) { this.defaultBoolValue = defaultBoolValue; }

        public PreBuildCheck(float defaultFloatValue) { this.defaultFloatValue = defaultFloatValue; }

        public PreBuildCheck(int defaultIntValue) { this.defaultIntValue = defaultIntValue; }

        public PreBuildCheck(long defaultLongValue) { this.defaultLongValue = defaultLongValue; }

        public PreBuildCheck(double defaultDoubleValue) { this.defaultDoubleValue = defaultDoubleValue; }

        public PreBuildCheck(string defaultStringValue) { this.defaultStringValue = defaultStringValue; }

        public bool defaultBoolValue;
        public float defaultFloatValue;
        public string defaultStringValue;
        public int defaultIntValue;
        public long defaultLongValue;
        public double defaultDoubleValue;
    }

#if UNITY_EDITOR

    [InitializeOnLoad]
    public class CustomBuildHandler
    {
        static CustomBuildHandler() { BuildPlayerWindow.RegisterBuildPlayerHandler(BuildWarningWizard.CreateWizard); }
    }

    public class BuildWarningWizard : ScriptableObject
    {
        public static void CreateWizard(BuildPlayerOptions obj)
        {
            string variables = CheckScriptsForAttributedVariables();
            GameObject[] gameObjects = Resources.FindObjectsOfTypeAll(typeof(GameObject)) as GameObject[];
            string activeGo = CheckGameObjects(gameObjects, "checkActive", true);
            string nonActiveGo = CheckGameObjects(gameObjects, "checkNonActive", false);
            if (variables != "" || activeGo != "" || nonActiveGo != "")
            {
                if (EditorUtility.DisplayDialog("Build warning",
                    (variables != ""
                        ? "Variables have test values:\n " + variables.Remove(variables.Length - 1, 1) + "\n"
                        : "") +
                    (activeGo != ""
                        ? "GameObjects are not active:\n " + activeGo.Remove(activeGo.Length - 1, 1) + "\n"
                        : "") +
                    (nonActiveGo != ""
                        ? "GameObjects are active:\n " + nonActiveGo.Remove(nonActiveGo.Length - 1, 1) + "\n"
                        : "") +
                    "Do you really want to continue ?",
                    "Yes", "No"))
                {
                    BuildPlayerWindow.DefaultBuildMethods.BuildPlayer(obj);
                }
                else
                {
                    throw new BuildPlayerWindow.BuildMethodException();
                }
            }
            else
            {
                BuildPlayerWindow.DefaultBuildMethods.BuildPlayer(obj);
            }
        }

        private static string CheckGameObjects(GameObject[] objects, string tag, bool isActive)
        {
            string active = "";
            if (objects == null) return "";
            foreach (var gameObject in objects)
            {
                if (gameObject.activeInHierarchy != isActive && gameObject.CompareTag(tag))
                {
                    active += gameObject.name + "\n ";
                }
            }

            return active;
        }

        private static string CheckScriptsForAttributedVariables()
        {
            ScriptableObject[] scriptables = GetAllInstances<ScriptableObject>();
            string variables = "";
            if (null == scriptables) return "";
            foreach (ScriptableObject mono in scriptables)
            {
                if (null == mono) continue;
                FieldInfo[] objectFields = mono.GetType().GetFields();
                for (int i = 0; i < objectFields.Length; i++)
                {
                    PreBuildCheck attribute =
                        Attribute.GetCustomAttribute(objectFields[i], typeof(PreBuildCheck)) as PreBuildCheck;
                    if (null != attribute)
                    {
                        if (objectFields[i].FieldType == typeof(string))
                        {
                            if (attribute.defaultStringValue != null &&
                                (string) objectFields[i].GetValue(mono) != attribute.defaultStringValue)
                            {
                                variables += objectFields[i].Name + "\n ";
                            }
                        }

                        if (objectFields[i].FieldType == typeof(bool))
                        {
                            if ((bool) objectFields[i].GetValue(mono) != attribute.defaultBoolValue)
                            {
                                variables += objectFields[i].Name + "\n ";
                            }
                        }

                        if (objectFields[i].FieldType == typeof(float))
                        {
                            if (Math.Abs((float) objectFields[i].GetValue(mono) - attribute.defaultFloatValue) >
                                Mathf.Epsilon)
                            {
                                variables += objectFields[i].Name + "\n ";
                            }
                        }

                        if (objectFields[i].FieldType == typeof(int))
                        {
                            if ((int) objectFields[i].GetValue(mono) != attribute.defaultIntValue)
                            {
                                variables += objectFields[i].Name + "\n ";
                            }
                        }

                        if (objectFields[i].FieldType == typeof(long))
                        {
                            if ((long) objectFields[i].GetValue(mono) != attribute.defaultLongValue)
                            {
                                variables += objectFields[i].Name + "\n ";
                            }
                        }

                        if (objectFields[i].FieldType == typeof(double))
                        {
                            if (Math.Abs((double) objectFields[i].GetValue(mono) - attribute.defaultDoubleValue) >
                                Mathf.Epsilon)
                            {
                                variables += objectFields[i].Name + "\n ";
                            }
                        }
                    }
                }
            }

            return variables;
        }

        private static T[] GetAllInstances<T>() where T : ScriptableObject
        {
            string[]
                guids = AssetDatabase.FindAssets("t:" + typeof(T)
                                                     .Name); //FindAssets uses tags check documentation for more info
            T[] a = new T[guids.Length];
            for (int i = 0; i < guids.Length; i++) //probably could get optimized 
            {
                string path = AssetDatabase.GUIDToAssetPath(guids[i]);
                a[i] = AssetDatabase.LoadAssetAtPath<T>(path);
            }

            return a;
        }
    }
}

#endif