using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Bremsengine
{
    #region Undo
    public partial class ProjectileGraphSO
    {
#if UNITY_EDITOR
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
#endif
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
    [CreateAssetMenu(menuName = "Bremsengine/Projectile2/Graph")]
    public partial class ProjectileGraphSO : ScriptableObject
    {
        public List<ProjectileNodeSO> nodes = new();
        public List<Projectile> SpawnGraph(Transform owner, Transform target, Vector2 fallbackPosition)
        {
            List<Projectile> spawns = new List<Projectile>();
            foreach (ProjectileNodeSO node in nodes)
            {
                foreach (var item in node.Spawn(owner, target, fallbackPosition))
                {
                    if (item == null)
                        continue;
                    spawns.Add(item);
                }
            }
            return spawns;
        }
    }
}