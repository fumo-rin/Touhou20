using Core.Extensions;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Bremsengine
{
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
        public void StartLine(Vector2 position)
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
            foreach(ProjectileEmitterSO emitter in emitters)
            {
                foreach (var link in emitter.linkedNodes)
                {
                    DrawLink(emitter, link);
                }
            }
            foreach (ProjectileGraphComponent c in components)
            {
                c.Draw(style);
                ProjectileGraphEditor.DrawHeader(c.rect, c.GetGraphComponentName());
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
            components.Remove(c);
            UndoStack.Push(c);
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
    #region Graph Components
    public partial class ProjectileGraphSO
    {
        public List<ProjectileGraphComponent> components = new();
    }
    #endregion
    [CreateAssetMenu(menuName = "Bremsengine/Projectile2/Graph")]
    public partial class ProjectileGraphSO : ScriptableObject
    {
        public bool Developing;
        public List<ProjectileNodeSO> nodes = new();
        public List<ProjectileEmitterSO> emitters = new();
        /// <summary>
        /// Should be Called as Projectile.SpawnProjectileGraph
        /// </summary>
        /// <param name="owner"></param>
        /// <param name="target"></param>
        /// <param name="fallbackPosition"></param>
        /// <returns></returns>
        public void SpawnGraph(ProjectileGraphInput input, Projectile.SpawnCallback callback)
        {
            Debug.Log("Shoot Graph : " + this.name);
            TriggeredEvent projectileEvents = new TriggeredEvent();
            /*foreach (ProjectileNodeSO node in nodes)
            {
                if (!node.NodeActive)
                    continue;
                //node.Spawn(spawns, input, projectileEvents);
                //callback?.Invoke(, input.Owner, input.Target);
            }*/
            Debug.Log(emitters);
            Debug.Log(emitters.Count);
            foreach (ProjectileEmitterSO emitter in emitters)
            {
                emitter.Trigger(projectileEvents, input, callback);
            }
        }
    }
}