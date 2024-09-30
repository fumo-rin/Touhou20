using System.Collections;
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
    }
}