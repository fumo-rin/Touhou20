using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Bremsengine
{
    #region Editor
#if UNITY_EDITOR
    public partial class ProjectileNodeSO
    {
        [HideInInspector] public Rect rect;
        private static List<ProjectileTypeSO> ProjectileCache => ProjectileGraphEditor.Cache;
        public void Initialize(Rect rect, ProjectileGraphSO graph, ProjectileTypeSO type)
        {
            this.graph = graph;
            this.name = "Projectile Node";
            this.ID = Guid.NewGuid().ToString();
            this.rect = rect;
            this.ProjectileType = type;
            graph.nodes.Add(this);
        }
        public void Draw(GUIStyle style)
        {
            GUILayout.BeginArea(rect, style);
            EditorGUI.BeginChangeCheck();

            int selected = ProjectileCache.FindIndex(x => x == ProjectileType);
            int selection = EditorGUILayout.Popup("", selected, GetProjectileTypesToDisplay());
            ProjectileType = ProjectileCache[selection];

            if (EditorGUI.EndChangeCheck())
                EditorUtility.SetDirty(this);
            GUILayout.EndArea();
        }
        public string[] GetProjectileTypesToDisplay()
        {
            string[] projectileArray = new string[ProjectileCache.Count];

            for (int i = 0; i < ProjectileCache.Count; i++)
            {
                projectileArray[i] = ProjectileCache[i].name;
            }
            return projectileArray;
        }
    }
#endif
    #endregion
    public partial class ProjectileNodeSO : ScriptableObject
    {
        public string ID;
        public ProjectileTypeSO ProjectileType;

        [HideInInspector] public ProjectileGraphSO graph;

    }
}
