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
        public void DrawLink(ProjectileNodeSO node, ProjectileEventSO e)
        {
            if (node == null || e == null)
            {
                return;
            }
            DrawLine(node.rect, e.rect);
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
            foreach (ProjectileNodeSO node in nodes)
            {
                node.Draw(style);
                ProjectileGraphEditor.DrawHeader(node.rect, node.GetHeaderName());
            }
            foreach (ProjectileEventSO e in knownEvents)
            {
                e.Draw(style);
                ProjectileGraphEditor.DrawHeader(e.rect, e.GetEventName());
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
        public Stack<ProjectileNodeSO> UndoStack = new Stack<ProjectileNodeSO>();
        public bool CanUndo => UndoStack != null && UndoStack.Count > 0;
        public ProjectileNodeSO UndoLast()
        {
            if (!CanUndo)
                return null;
            ProjectileNodeSO undo = UndoStack.Pop();
            nodes.Add(undo);
            return undo;
        }
        public void RemoveAndAddToUndo(object node)
        {
            RemoveAndAddToUndo(node as ProjectileNodeSO);
        }
        public void RemoveAndAddToUndo(ProjectileNodeSO node)
        {
            if (node is not null and ProjectileNodeSO n)
            {
                nodes.Remove(n);
                UndoStack.Push(n);
            }
        }
    }
    #endregion
#endif
    #region Projectile Events
    public partial class ProjectileGraphSO
    {
        public List<ProjectileEventSO> knownEvents = new();
    }
    #endregion
    [CreateAssetMenu(menuName = "Bremsengine/Projectile2/Graph")]
    public partial class ProjectileGraphSO : ScriptableObject
    {
        public bool Developing;
        public List<ProjectileNodeSO> nodes = new();
        /// <summary>
        /// Should be Called as Projectile.SpawnProjectileGraph
        /// </summary>
        /// <param name="owner"></param>
        /// <param name="target"></param>
        /// <param name="fallbackPosition"></param>
        /// <returns></returns>
        public List<Projectile> SpawnGraph(Transform owner, Transform target, Vector2 fallbackPosition)
        {
            List<Projectile> spawns = new List<Projectile>();
            TriggeredEvent projectileEvents = new TriggeredEvent();
            foreach (ProjectileNodeSO node in nodes)
            {
                if (!node.NodeActive)
                    continue;
                node.Spawn(spawns, owner, target, fallbackPosition, projectileEvents);
            }
            BroadcastTriggeredEvents(projectileEvents);
            return spawns;
        }
        public static void BroadcastTriggeredEvents(TriggeredEvent triggeredEvents)
        {

        }
    }
}