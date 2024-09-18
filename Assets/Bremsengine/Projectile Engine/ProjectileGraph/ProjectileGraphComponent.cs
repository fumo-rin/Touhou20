using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Core.Extensions;
using Bremsengine;
namespace Bremsengine
{
    #region Editor
#if UNITY_EDITOR
    using UnityEditor;
    public abstract partial class ProjectileGraphComponent
    {
        [HideInInspector] public Rect rect;
        public abstract void OnDrag(Vector2 delta);
        protected abstract Rect GetRect(Vector2 mousePosition);
        public abstract string GetGraphComponentName();
        public abstract void OnGraphDelete();
        protected abstract void OnInitialize(Vector2 mousePosition, ProjectileGraphSO graph, ProjectileTypeSO type);
        protected abstract void OnDraw(GUIStyle style);
        public void Drag(Vector2 delta)
        {
            rect.position += delta;
            OnDrag(delta);
            GUI.changed = true;
            this.Dirty();
        }
        public void Initialize(Vector2 mousePosition, ProjectileGraphSO graph, ProjectileTypeSO type = null)
        {
            rect = GetRect(mousePosition);
            this.graph = graph;
            this.name = GetGraphComponentName();
            if (string.IsNullOrEmpty(ID))
            {
                this.ID = Guid.NewGuid().ToString();
            }
            graph.components.AddIfDoesntExist(this);
            OnInitialize(mousePosition, graph, type);
        }
        public bool IsMouseOver(Vector2 mousePosition)
        {
            return rect.Contains(mousePosition);
        }
        public void DeleteComponent()
        {
            OnGraphDelete();
            graph.components.Remove(this);
            AssetDatabase.SaveAssets();
        }
        public void Draw(GUIStyle style)
        {
            GUILayout.BeginArea(rect, style);
            EditorGUI.BeginChangeCheck();

            OnDraw(style);

            if (EditorGUI.EndChangeCheck())
            {
                EditorUtility.SetDirty(this);
                AssetDatabase.SaveAssetIfDirty(this);
            }
            GUILayout.EndArea();
            SecondaryDraw(style);
        }
        protected virtual void SecondaryDraw(GUIStyle style) { }
    }
#endif
    #endregion
    public abstract partial class ProjectileGraphComponent : ScriptableObject
    {
        public string ID;
        [HideInInspector] public ProjectileGraphSO graph;
    }
}