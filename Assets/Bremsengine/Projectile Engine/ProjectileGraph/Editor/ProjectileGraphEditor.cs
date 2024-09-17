using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Core;
using System.Linq;
using System;



#if UNITY_EDITOR
using UnityEditor.Callbacks;
namespace Bremsengine
{
    #region Double Click & Window
    public partial class ProjectileGraphEditor
    {
        private static ProjectileGraphSO ActiveGraph;
        [OnOpenAsset(0)]
        public static bool OnDoubleClickAsset(int instanceID, int line)
        {
            ProjectileGraphSO graph = EditorUtility.InstanceIDToObject(instanceID) as ProjectileGraphSO;
            if (graph != null)
            {
                OpenWindow();

                ActiveGraph = graph;
                return true;
            }
            return false;
        }

        [MenuItem("Projectile Editor", menuItem = "Bremsengine/Projectile Editor")]
        private static void OpenWindow()
        {
            GetWindow<ProjectileGraphEditor>("Projectile Editor");
        }
    }
    #endregion
    #region Load Projectiles
    public partial class ProjectileGraphEditor
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void ClearCache()
        {
            projectilePrefabs = null;
            LoadCache();
        }
        private static List<ProjectileTypeSO> projectilePrefabs;
        public static List<ProjectileTypeSO> ProjectileTypeLookup => LoadCache();
        private const string ProjectileAddressablesKey = "Projectile Types";
        private static List<ProjectileTypeSO> LoadCache()
        {
            if (projectilePrefabs == null)
            {
                projectilePrefabs = new List<ProjectileTypeSO>();
                foreach (var item in AddressablesTools.LoadKeys<ProjectileTypeSO>(ProjectileAddressablesKey).Where(x => x is ProjectileTypeSO and not null))
                {
                    projectilePrefabs.Add(item);
                }
            }
            if (projectilePrefabs == null || projectilePrefabs.Count <= 0)
            {
                Debug.LogWarning("projectiles arent initialized properly or doesnt exist");
                return null;
            }
            return projectilePrefabs;
        }
    }
    #endregion
    #region Grid
    public partial class ProjectileGraphEditor
    {
        Vector2 gridOffset;
        Vector2 gridDrag;
        const float gridLarge = 100f;
        const float gridSmall = 25f;

        bool isDraggingGrid;
        private void DragGrid(Vector2 delta)
        {
            if (isDraggingGrid)
            {
                foreach (var item in AllNodes)
                {
                    item.DragNode(delta);
                }
            }
        }
    }
    #endregion
    #region GUI
    public partial class ProjectileGraphEditor
    {
        private void OnGUI()
        {
            if (ActiveGraph == null)
            {
                return;
            }
            ProcessEvents(Event.current);
            DrawNodes();

            if (GUI.changed)
                Repaint();
        }
        private void DrawNodes()
        {
            foreach (ProjectileNodeSO node in ActiveGraph.nodes)
            {
                node.Draw(projectileNodeStyle);
            }
            GUI.changed = true;
        }
    }
    #endregion
    #region Events
    public partial class ProjectileGraphEditor
    {
        private static ProjectileNodeSO currentEventNode;
        #region Mouse Down
        private void ProcessMouseDownEvent(Event e)
        {
            bool StartDrag(Event e)
            {
                ForceEndDrag();
                if (e.button == 0)
                {
                    if (IsMouseOverNode(e, out ProjectileNodeSO drag))
                    {
                        if (drag != null)
                        {
                            dragNode = drag;
                        }
                    }
                }
                return dragNode != null;
            }
            void DragGrid(Event e)
            {
                if (e.button != 1 && !IsMouseOverNode(e, out _))
                {
                    isDraggingGrid = true;
                }
            }
            if (e.button == 1)
            {
                ShowContextMenu(e);
            }
            if (!StartDrag(e))
            {
                DragGrid(e);
            }
        }
        #endregion
        #region Mouse Up
        private void ProcessMouseUpEvent(Event e)
        {
            if (e.button == 0)
            {
                dragNode = null;
                isDraggingGrid = false;
            }
        }
        #endregion
        #region Mouse Drag
        private static ProjectileNodeSO dragNode;
        public static void ForceEndDrag()
        {
            dragNode = null;
        }
        private void ProcessMouseDrag(Event e)
        {
            if (dragNode)
            {
                dragNode.DragNode(e.delta);
            }
            DragGrid(e.delta);
        }
        #endregion
        private void ProcessEvents(Event e)
        {
            if (IsMouseOverNode(e, out currentEventNode))
            {
                currentEventNode.ProcessEvents(e);
            }
            ProcessProjectileGraphEvents(e);
        }
        private void ProcessProjectileGraphEvents(Event e)
        {
            switch (e.type)
            {
                case EventType.MouseDown:
                    ProcessMouseDownEvent(e);
                    break;
                case EventType.MouseUp:
                    ProcessMouseUpEvent(e);
                    break;
                case EventType.MouseDrag:
                    ProcessMouseDrag(e);
                    break;
                default:
                    break;
            }
        }
        private void ShowContextMenu(Event e)
        {
            GenericMenu menu = new GenericMenu();
            Vector2 position = e.mousePosition;
            ProjectileNodeSO mouseOver = null;
            if (IsMouseOverNode(e, out mouseOver))
            {
                menu.AddItem(new GUIContent("Remove Projectile Node"), false, DestroyNode, mouseOver);
            }
            if (mouseOver == null)
            {
                menu.AddItem(new GUIContent("Add Single Projectile"), false, AddSingleProjectile, position);
                menu.AddItem(new GUIContent("Add Projectile Arc"), false, AddProjectileArc, position);
                if (ActiveGraph.CanUndo)
                {
                    menu.AddItem(new GUIContent("Undo Delete"), false, UndoLastDelete);
                }
            }
            menu.ShowAsContext();
        }
        private bool IsMouseOverNode(Event e, out ProjectileNodeSO node)
        {
            node = null;
            for (int i = 0; i < ActiveGraph.nodes.Count; i++)
            {
                if (ActiveGraph.nodes[i].rect.Contains(e.mousePosition))
                {
                    node = ActiveGraph.nodes[i];
                    break;
                }
            }
            return node != null;
        }
    }
    #endregion
    #region Projectile Nodes
    public partial class ProjectileGraphEditor
    {
        private List<ProjectileNodeSO> AllNodes => ActiveGraph.nodes;
        #region Add Single Projectile
        private void AddSingleProjectile(object mousePositionObject)
        {
            ProjectileNodeSO newNode = null;
            Vector2 mousePosition = (Vector2)mousePositionObject;
            newNode = ScriptableObject.CreateInstance<SingleProjectileNodeSO>();
            if (newNode != null)
            {
                newNode.Initialize(NodeRect(mousePosition.x, mousePosition.y, 160f, 75f), ActiveGraph, projectilePrefabs[0]);

                AssetDatabase.AddObjectToAsset(newNode, ActiveGraph);
                AssetDatabase.SaveAssets();
            }
        }
        #endregion
        #region Add Projectile Arc
        private void AddProjectileArc(object mousePositionObject)
        {
            ProjectileNodeSO newNode = null;
            Vector2 mousePosition = (Vector2)mousePositionObject;
            newNode = ScriptableObject.CreateInstance<ProjectileArcNodeSO>();
            if (newNode != null)
            {
                newNode.Initialize(NodeRect(mousePosition.x, mousePosition.y, 160f, 75f), ActiveGraph, projectilePrefabs[0]);

                AssetDatabase.AddObjectToAsset(newNode, ActiveGraph);
                AssetDatabase.SaveAssets();
            }
        }
        #endregion
        #region Destroy & Undo Destroy Node
        private void DestroyNode(object node)
        {
            ActiveGraph.RemoveAndAddToUndo(node);
        }
        private void UndoLastDelete()
        {
            ActiveGraph.UndoLast();
        }
        #endregion
    }
    #endregion
    public partial class ProjectileGraphEditor : UnityEditor.EditorWindow
    {
        private GUIStyle projectileNodeStyle;

        private const int nodePadding = 25;
        private const int nodeBorder = 12;

        private void OnEnable()
        {
            projectileNodeStyle = new GUIStyle();
            projectileNodeStyle.normal.textColor = Color.white;
            projectileNodeStyle.normal.background = EditorGUIUtility.Load("node1") as Texture2D;
            projectileNodeStyle.padding = new RectOffset(nodePadding, nodePadding, nodePadding, nodePadding);
            projectileNodeStyle.border = new RectOffset(nodeBorder, nodeBorder, nodeBorder, nodeBorder);
        }
        public static Rect NodeRect(float x, float y, float w, float h)
        {
            return new Rect(new Vector2(x, y), new Vector2(w, h));
        }
    }
}
#endif