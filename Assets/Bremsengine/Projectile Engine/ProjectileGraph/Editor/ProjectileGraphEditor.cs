using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Core;
using System.Linq;
using System;
using Core.Extensions;
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
                ActiveGraph.SetActiveGraph();
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
                foreach (var item in ActiveGraph.components)
                {
                    item.Drag(delta);
                }
                ActiveGraph.DragPreviewLine(delta);
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
            ActiveGraph.Draw(projectileNodeStyle);
            GUI.changed = true;
        }
        public static void DrawHeader(Rect rect, string title, float height = 40f)
        {
            float width = rect.width - 25f;
            Rect header = new(new(rect.center.x - (width * 0.5f), rect.center.y + -(rect.height * 0.5f) - height), new(width, height));
            GUILayout.BeginArea(header, title, HeaderStyle);
            //EditorGUILayout.LabelField(title);
            GUILayout.EndArea();
        }
    }
    #endregion
    #region Input Events
    public partial class ProjectileGraphEditor
    {
        ProjectileEventSO projectileEventDragSelection;
        static bool CursorMoveDrag;
        static Vector2 DragLinePreviewStart;
        #region Mouse Down
        private void ProcessMouseDownEvent(Event e)
        {
            bool StartDragItem(Event e)
            {
                ForceEndDrag();
                if (e.button == 2)
                {
                    if (IsMouseOverNode(e, out ProjectileNodeSO drag))
                    {
                        if (drag != null)
                        {
                            dragNode = drag;
                            CursorMoveDrag = true;
                            return true;
                        }
                    }
                    if (IsMouseOverProjectileEvent(e, out projectileEventDragSelection))
                    {
                        if (projectileEventDragSelection != null)
                        {
                            CursorMoveDrag = true;
                            return true;
                        }
                    }
                }
                if (e.button == 0)
                {
                    if (IsMouseOverProjectileEvent(e, out projectileEventDragSelection))
                    {
                        DragLinePreviewStart = e.mousePosition;
                        ActiveGraph.StartLine(e.mousePosition);
                    }
                }
                return false;
            }
            void TryDragGrid(Event e)
            {
                if (e.button == 2 && !IsMouseOverNode(e, out _) && !IsMouseOverProjectileEvent(e, out _))
                {
                    isDraggingGrid = true;
                }
            }
            if (e.button == 1)
            {
                ShowContextMenu(e);
            }
            if (!StartDragItem(e))
            {
                TryDragGrid(e);
            }
        }
        #endregion
        #region Mouse Up
        private void ProcessMouseUpEvent(Event e)
        {
            CursorMoveDrag = false;
            if (e.button == 2)
            {
                dragNode = null;
                isDraggingGrid = false;
            }
            if (e.button == 0)
            {
                if (projectileEventDragSelection != null && IsMouseOverNode(e, out ProjectileNodeSO hover))
                {
                    EditorGUI.BeginChangeCheck();

                    Debug.Log("Add event: " + projectileEventDragSelection.name + " to :" + hover.name);
                    hover.linkedProjectileEvents.AddIfDoesntExist(projectileEventDragSelection);
                    EditorUtility.SetDirty(hover);
                    EditorUtility.SetDirty(ActiveGraph);

                    if (EditorGUI.EndChangeCheck())
                    {
                        EditorUtility.SetDirty(this);
                        AssetDatabase.SaveAssetIfDirty(this);
                    }
                }
                if (ActiveGraph.previewLine && IsMouseOverNode(e, out ProjectileNodeSO lineToNode))
                {

                }
                ActiveGraph.EndPreviewLine();
            }
            projectileEventDragSelection = null;
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
            if (CursorMoveDrag)
            {
                if (dragNode)
                {
                    dragNode.Drag(e.delta);
                }
                if (projectileEventDragSelection)
                {
                    projectileEventDragSelection.DragEvent(e.delta);
                }
            }
            DragGrid(e.delta);
        }
        #endregion
        #region Process Events
        private void ProcessEvents(Event e)
        {
            ProcessProjectileGraphEvents(e);
            ActiveGraph.ProcessPreviewLine(e);
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
        #endregion
        #region Show Context Menu
        private void ShowContextMenu(Event e)
        {
            GenericMenu menu = new GenericMenu();
            Vector2 position = e.mousePosition;
            ProjectileNodeSO mouseOverNode = null;
            ProjectileEventSO mouseOverProjectileEvent = null;
            if (IsMouseOverNode(e, out mouseOverNode))
            {
                if (ActiveGraph.Developing)
                {
                    menu.AddItem(new GUIContent("(Developing) Re Initialize Node"), false, mouseOverNode.Reinitalize);
                }
            }
            if (IsMouseOverProjectileEvent(e, out mouseOverProjectileEvent))
            {
                if (ActiveGraph.Developing)
                {
                    menu.AddItem(new GUIContent("(Developing) Re Initialize Event"), false, mouseOverProjectileEvent.Reinitialize);
                }
            }
            if (IsMouseOverComponent(e, out ProjectileGraphComponent c))
            {
                menu.AddItem(new GUIContent("Remove Projectile Node"), false, ActiveGraph.DestroyComponent, c);
            }
            bool anyMouseOver = mouseOverNode != null || mouseOverProjectileEvent != null;
            if (mouseOverNode == null)
            {
                menu.AddItem(new GUIContent("Add Single Projectile"), false, AddSingleProjectile, position);
                menu.AddItem(new GUIContent("Add Projectile Arc"), false, AddProjectileArc, position);
                if (ActiveGraph.CanUndo)
                {
                    menu.AddItem(new GUIContent("Undo Delete"), false, ActiveGraph.UndoLastDelete);
                }
            }
            if (mouseOverProjectileEvent == null)
            {
                menu.AddItem(new GUIContent("Add Play Sound Event"), false, AddPlaySoundEvent, position);
                menu.AddItem(new GUIContent("Add Crawler Event"), false, AddCrawlerEvent, position);
            }
            menu.ShowAsContext();
        }
        #endregion
        #region Mouse Over Projectile Event
        private bool IsMouseOverComponent(Event e, out ProjectileGraphComponent c)
        {
            c = null;
            for (int i = 0; i < ActiveGraph.components.Count; i++)
            {
                if (ActiveGraph.components[i].IsMouseOver(e.mousePosition) && ActiveGraph.components[i] is ProjectileGraphComponent found and not null)
                {
                    c = found;
                    break;
                }
            }
            return c != null;
        }
        private bool IsMouseOverProjectileEvent(Event e, out ProjectileEventSO foundEvent)
        {
            foundEvent = null;
            for (int i = 0; i < ActiveGraph.components.Count; i++)
            {
                if (ActiveGraph.components[i].IsMouseOver(e.mousePosition) && ActiveGraph.components[i] is ProjectileEventSO found and not null)
                {
                    foundEvent = found;
                    break;
                }
            }
            return foundEvent != null;
        }
        #endregion
        #region Mouse Over Node
        private bool IsMouseOverNode(Event e, out ProjectileNodeSO node)
        {
            node = null;
            for (int i = 0; i < ActiveGraph.nodes.Count; i++)
            {
                if (ActiveGraph.nodes[i].rect.Contains(e.mousePosition) && ActiveGraph.nodes[i] is ProjectileNodeSO found and not null)
                {
                    node = ActiveGraph.nodes[i];
                    break;
                }
            }
            return node != null;
        }
        #endregion
    }
    #endregion
    #region Projectile Nodes & Events
    public partial class ProjectileGraphEditor
    {
        #region Add Single Projectile
        private void AddSingleProjectile(object mousePositionObject)
        {
            LoadCache();
            ProjectileNodeSO newNode = null;
            Vector2 mousePosition = (Vector2)mousePositionObject;
            newNode = ScriptableObject.CreateInstance<SingleProjectileNodeSO>();
            if (newNode != null)
            {
                Debug.Log(ActiveGraph);
                Debug.Log(projectilePrefabs.Count);
                Debug.Log(newNode);
                newNode.Initialize(mousePosition, ActiveGraph, projectilePrefabs[0]);

                AssetDatabase.AddObjectToAsset(newNode, ActiveGraph);
                AssetDatabase.SaveAssets();
            }
        }
        #endregion
        #region Add Projectile Arc
        private void AddProjectileArc(object mousePositionObject)
        {
            LoadCache();
            ProjectileNodeSO newNode = null;
            Vector2 mousePosition = (Vector2)mousePositionObject;
            newNode = ScriptableObject.CreateInstance<ProjectileArcNodeSO>();
            if (newNode != null)
            {
                newNode.Initialize(mousePosition, ActiveGraph, projectilePrefabs[0]);

                AssetDatabase.AddObjectToAsset(newNode, ActiveGraph);
                AssetDatabase.SaveAssets();
            }
        }
        #endregion
        #region Add Play Sound Event
        private void AddPlaySoundEvent(object mousePosition)
        {
            LoadCache();
            ProjectileGraphComponent newEvent = null;
            newEvent = ScriptableObject.CreateInstance<PlaySoundEventSO>();
            if (newEvent != null)
            {
                newEvent.Initialize((Vector2)mousePosition, ActiveGraph);

                AssetDatabase.AddObjectToAsset(newEvent, ActiveGraph);
                AssetDatabase.SaveAssets();
            }
        }
        private void AddCrawlerEvent(object mousePosition)
        {
            LoadCache();
            ProjectileEventSO newEvent = null;
            newEvent = ScriptableObject.CreateInstance<CrawlerEventSO>();
            if (newEvent != null)
            {
                newEvent.Initialize((Vector2)mousePosition, ActiveGraph);

                AssetDatabase.AddObjectToAsset(newEvent, ActiveGraph);
                AssetDatabase.SaveAssets();
            }
        }
        #endregion
        #region Add Crawler Event

        #endregion
    }
    #endregion
    #region Drag
    public partial class ProjectileNodeSO
    {
        public override void OnDrag(Vector2 delta)
        {
            projectileImagePreview.position += delta;
        }
    }
    public partial class ProjectileEventSO
    {
        public void DragEvent(Vector2 delta)
        {
            rect.position += delta;
            GUI.changed = true;
            this.Dirty();
        }
    }
    #endregion
    public partial class ProjectileGraphEditor : UnityEditor.EditorWindow
    {
        public static void SetHeaderTexture(Texture2D tex)
        {
            int headerPaddingX = 10;
            int headerPaddingY = 5;
            int border = 12;
            HeaderStyle = new();
            HeaderStyle.normal.textColor = Color.white;
            HeaderStyle.normal.background = TextureOrFallback(tex);
            HeaderStyle.padding = new RectOffset(headerPaddingX, headerPaddingY, headerPaddingX, 0);
            HeaderStyle.border = new RectOffset(border, border, border, border);
            HeaderStyle.richText = true;
            HeaderStyle.fontSize = 16;
        }
        private static Texture2D headerTexture;
        private GUIStyle projectileNodeStyle;
        public static GUIStyle HeaderStyle;
        private static Texture2D cachedFallbackTexture;
        public static Texture2D FallbackTexture => cachedFallbackTexture == null ? cachedFallbackTexture = EditorGUIUtility.Load("node1") as Texture2D : cachedFallbackTexture;

        private const int nodePadding = 25;
        private const int nodeBorder = 12;
        public static Texture2D TextureOrFallback(Texture2D tex)
        {
            return tex == null ? FallbackTexture : tex;
        }
        private void OnEnable()
        {
            projectileNodeStyle = new GUIStyle();
            projectileNodeStyle.normal.textColor = Color.white;
            projectileNodeStyle.normal.background = TextureOrFallback(null);
            projectileNodeStyle.padding = new RectOffset(nodePadding, nodePadding, nodePadding, nodePadding);
            projectileNodeStyle.border = new RectOffset(nodeBorder, nodeBorder, nodeBorder, nodeBorder);

            SetHeaderTexture(null);
        }
        public static void SetDirty(UnityEngine.Object target)
        {
            EditorUtility.SetDirty(target);
        }
    }
    #region Helper
    public static partial class ProjectileGraphHelper
    {
        public static void Dirty(this UnityEngine.Object o) => EditorUtility.SetDirty(o);
    }
    #endregion
}
#endif