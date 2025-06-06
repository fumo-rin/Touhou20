using System;
using System.Collections;
using UnityEditor;
using UnityEngine;

namespace Core.Extensions
{
    public static partial class Helper
    {
        public static void EditorPing(this UnityEngine.Object o)
        {
#if UNITY_EDITOR
            UnityEditor.EditorGUIUtility.PingObject(o);
#endif
        }
        public static void Repaint()
        {
#if UNITY_EDITOR
            SceneView.RepaintAll();
#endif
        }
        public static void SetDirtyAndSave(this UnityEngine.Object o)
        {
#if UNITY_EDITOR
            EditorUtility.SetDirty(o);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
#endif
        }
        public static void Dirty(this UnityEngine.Object o)
        {
#if UNITY_EDITOR
            EditorUtility.SetDirty(o);
#endif
        }
        public static T Spawn2D<T>(this T reference, Vector2 position) where T : UnityEngine.MonoBehaviour
        {
            if (reference == null)
            {
                return default(T);
            }
            return MonoBehaviour.Instantiate(reference, position, Quaternion.identity);
        }
    }
}