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
    #region Base Class
    public abstract partial class ProjectileGraphComponent
    {
        [HideInInspector] public Rect rect;
        public abstract void OnDrag(Vector2 delta);
        protected abstract Rect GetRect(Vector2 mousePosition);
        public abstract string GetGraphComponentName();
        public abstract void OnGraphDelete();
        protected abstract void OnInitialize(Vector2 mousePosition, ProjectileGraphSO graph, ProjectileTypeSO type);
        protected abstract void OnDraw(GUIStyle style);
        public abstract void TryBreakLinks();
        public abstract void ReceiveBroadcastUnlink(ProjectileGraphComponent unlink);
        public void Drag(Vector2 delta)
        {
            rect.position += delta;
            OnDrag(delta);
            GUI.changed = true;
            this.Dirty();
        }
        public virtual bool TryStartLink(out ProjectileGraphComponent linkStart)
        {
            linkStart = this;
            return true;
        }
        public virtual bool TryEndLink(out ProjectileGraphComponent linkEnd)
        {
            linkEnd = this;
            return true;
        }
        public abstract void TryCreateLink(ProjectileGraphComponent other);
        public void RecalculateRect()
        {
            if (rect == null)
            {
                Debug.LogError("Bad");
                return;
            }
            rect = GetRect(rect.position);
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
        public void Draw(GUIStyle style)
        {
            GUILayout.BeginArea(rect, style);
            EditorGUI.BeginChangeCheck();

            OnDraw(style);

            if (EditorGUI.EndChangeCheck())
            {
                EditorUtility.SetDirty(this);
            }
            GUILayout.EndArea();
            SecondaryDraw(style);
        }
        protected virtual void SecondaryDraw(GUIStyle style) { }
    }
    #endregion
    #region Functions
    public abstract partial class ProjectileGraphComponent
    {
        protected void DrawLabel(string label, bool hasSpaceAbove = true)
        {
            if (hasSpaceAbove)
                AddSpace(1);
            EditorGUILayout.LabelField(label);
            AddSpace(1);
        }
        public bool IsMouseOver(Vector2 mousePosition)
        {
            return rect.Contains(mousePosition);
        }
        public void DeleteComponent()
        {
            TryBreakLinks();
            OnGraphDelete();
            graph.Dirty();
            foreach (var item in graph.components)
            {
                if (item == this)
                    continue;
                item.ReceiveBroadcastUnlink(this);
            }
            graph.components.Remove(this);
            graph.nodes.Remove(this as ProjectileNodeSO);
            graph.emitters.Remove(this as ProjectileEmitterSO);
            graph.modNodes.Remove(this as ProjectileModNodeSO);
            graph.componentSelectors.Remove(this as ProjectileComponentSelector);
            AssetDatabase.RemoveObjectFromAsset(this);
            AssetDatabase.SaveAssets();
        }
        public void AddSpace(int count)
        {
            for (int i = 0; i < count; i++)
            {
                EditorGUILayout.Space();
            }
        }
        public void Reinitialize()
        {
            OnInitialize(new(rect.x, rect.y), graph, null);
        }
    }
    #endregion
#endif
    #endregion
    public abstract partial class ProjectileGraphComponent : ScriptableObject
    {
        public string ID;
        [HideInInspector] public ProjectileGraphSO graph;
        protected Projectile CreateProjectile(Projectile p, Vector2 position, ProjectileNodeDirection direction, float directionalOffset, float spread, float addedAngle, float speed)
        {
            direction.SetSpeed(speed * graph.GetGlobalSpeed());
            direction.AddAngle(addedAngle);
            direction.SetSpread(spread);
            direction.SetDirectionalOffset(directionalOffset);
            Projectile spawnProjectile = Projectile.NewCreateFromQueue(p, position, direction).SetDamage(1f);
            return spawnProjectile;
        }
    }
}