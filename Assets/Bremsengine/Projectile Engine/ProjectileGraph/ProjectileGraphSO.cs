using Core.Extensions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Bremsengine
{
    #region Graph Components
    public partial class ProjectileGraphSO
    {
        public List<ProjectileGraphComponent> components = new();
#if UNITY_EDITOR
        [ContextMenu("Recalculate All Rects")]
        public void RecalculateAllRect()
        {
            foreach (var item in components)
            {
                item.RecalculateRect();
            }
        }
#endif
    }
    #endregion
    [CreateAssetMenu(menuName = "Bremsengine/Projectile2/Graph")]
    public partial class ProjectileGraphSO : ScriptableObject
    {
        public string projectileGraphName;
        public bool Developing;
        public List<ProjectileNodeSO> nodes = new();
        public List<ProjectileEmitterSO> emitters = new();
        public List<ProjectileModNodeSO> modNodes = new();
        public List<ProjectileComponentSelector> componentSelectors = new();
        [Range(0.1f, 5f)][SerializeField] float GraphGlobalProjectileSpeed = 1f;
        [Range(-10f, 10f)]
        [SerializeField] float addedCooldown = 1f;
        public float CalculateCooldown(float externalCooldownInSeconds)
        {
            float highestCooldown = 0f;
            foreach (var item in emitters)
            {
                if (item.CooldownDuration > highestCooldown)
                {
                    highestCooldown = item.CooldownDuration;
                }
            }
            return addedCooldown + externalCooldownInSeconds + highestCooldown;
        }
        public float GetGlobalSpeed()
        {
            return GraphGlobalProjectileSpeed;
        }
        /// <summary>
        /// Should be Called as Projectile.SpawnProjectileGraph
        /// </summary>
        /// <param name="owner"></param>
        /// <param name="target"></param>
        /// <param name="fallbackPosition"></param>
        /// <returns></returns>
        public void SpawnGraph(ProjectileGraphInput input, Projectile.SpawnCallback callback, int forcedLayer)
        {
            TriggeredEvent projectileEvents = new TriggeredEvent();
            /*foreach (ProjectileNodeSO node in nodes)
            {
                if (!node.NodeActive)
                    continue;
                //node.Spawn(spawns, input, projectileEvents);
                //callback?.Invoke(, input.Owner, input.Target);
            }*/
            foreach (ProjectileEmitterSO emitter in emitters.Where(x => x.Active))
            {
                emitter.Trigger(projectileEvents, input, callback, forcedLayer);
            }
        }
    }
#if UNITY_EDITOR
    public partial class ProjectileGraphSO
    {
        public void SetActiveGraph()
        {
            ProjectileGraphEditor.SetHeaderTexture(headerTexture);
        }
        [SerializeField] public Texture2D headerTexture;
        public bool previewLine;
        static Vector2 previewLinePosition;
        static Vector2 previewLineEnd;
        [SerializeField] float connectingLineWidth = 3f;
        public void DragPreviewLine(Vector2 delta)
        {
            previewLinePosition += delta;
        }
        public void DrawLink(ProjectileGraphComponent a, ProjectileGraphComponent b)
        {
            if (a == null || b == null)
            {
                return;
            }
            DrawLine(a.rect, b.rect);
        }
        public void DrawLine(Rect a, Rect b)
        {
            DrawLine(a.center, b.center);
        }
        private void DrawLine(Vector2 start, Vector2 target)
        {
            Handles.DrawBezier(start, target, start, target, ColorHelper.Peach, null, connectingLineWidth);
        }
        public void ProcessPreviewLine(Event e)
        {
            previewLineEnd = e.mousePosition;
        }
        public void EndPreviewLine()
        {
            Debug.Log("End Pleae");
            previewLine = false;
        }
        public void StartPreviewLine(Vector2 position)
        {
            Debug.Log("Start Line");
            previewLinePosition = position;
            previewLineEnd = position;
            previewLine = true;
        }
    }
    #region Draw
    public partial class ProjectileGraphSO
    {
        public void Draw(GUIStyle style)
        {
            if (previewLine)
            {
                DrawLine(previewLinePosition, previewLineEnd);
            }
            foreach (ProjectileNodeSO node in nodes)
            {
                foreach (var link in node.linkedProjectileEvents)
                {
                    DrawLink(node, link);

                }
            }
            foreach (ProjectileEmitterSO emitter in emitters)
            {
                foreach (var link in emitter.linkedNodes)
                {
                    DrawLink(emitter, link);
                }
                DrawLink(emitter, emitter.linkedOverrideDirection);
            }
            foreach (ProjectileModNodeSO mod in modNodes)
            {
                foreach (var item in mod.attachedNodes)
                {
                    DrawLink(item, mod);
                }
            }
            for (int i = 0; i < components.Count; i++)
            {
                ProjectileGraphComponent c = components[i];
                if (components[i] == null)
                {
                    i--;
                    continue;
                }
                ProjectileGraphEditor.DrawHeader(c.rect, c.GetGraphComponentName());
                c.Draw(style);
            }
        }
    }
    #endregion
    #region Undo
    public partial class ProjectileGraphSO
    {
        [ContextMenu("Clear Undo Delete Stack")]
        public void ClearUndoStack()
        {
            foreach (var item in UndoStack)
            {
                DestroyImmediate(item, true);
            }
            UndoStack.Clear();
            AssetDatabase.SaveAssets();
        }
        private Stack<ProjectileGraphComponent> UndoStack = new Stack<ProjectileGraphComponent>();
        public bool CanUndo => UndoStack != null && UndoStack.Count > 0;
        private ProjectileGraphComponent UndoLast()
        {
            if (!CanUndo)
                return null;
            ProjectileGraphComponent undo = UndoStack.Pop();
            if (undo is ProjectileNodeSO node)
            {
                nodes.Add(node);
            }
            components.Add(undo);
            return undo;
        }
        private void RemoveAndAddToUndo(ProjectileGraphComponent c)
        {
            if (c is not null and ProjectileNodeSO n)
            {
                nodes.Remove(n);
            }
            if (c is not null and ProjectileModNodeSO m)
            {
                modNodes.Remove(m);
            }
            components.Remove(c);
            UndoStack.Push(c);
            c.DeleteComponent();
            c.OnGraphDelete();
        }
    }
    #endregion
    #region Destroy & Undo Destroy Node
    public partial class ProjectileGraphSO
    {
        public void DestroyComponent(object node)
        {
            RemoveAndAddToUndo((ProjectileGraphComponent)node);
        }
        public void UndoLastDelete()
        {
            UndoLast();
        }
    }
    #endregion
#endif
}