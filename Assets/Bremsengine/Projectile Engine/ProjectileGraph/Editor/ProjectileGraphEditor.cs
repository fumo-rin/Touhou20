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
    #region Context Menu
    public partial class ProjectileGraphEditor
    {
        private void ShowContextMenu(Event e)
        {
            GenericMenu menu = new GenericMenu();
            Vector2 position = e.mousePosition;
            ProjectileGraphComponent hover = null;
            if (IsMouseOverComponent(e, out hover))
            {

            }
            if (hover != null)
            {
                menu.AddItem(new GUIContent("Destroy Copmonent"), false, hover.DeleteComponent);
                menu.AddItem(new GUIContent("Try Break Links"), false, hover.TryBreakLinks);
                menu.AddItem(new GUIContent("Reinitialize"), false, hover.Reinitialize);
                menu.AddItem(new GUIContent("Redraw Box"), false, hover.RecalculateRect);
            }
            menu.AddItem(new GUIContent("Add Component"), false, AddSelector, position);
            menu.ShowAsContext();
            ActiveDrag = null;
        }
    }
    #endregion
    #region Double Click & Window
    public partial class ProjectileGraphEditor
    {
        private static Dictionary<string, ProjectileGraphSO> graphsCache;
        private static ProjectileGraphSO ActiveGraph;
        [OnOpenAsset(0)]
        public static bool OnDoubleClickAsset(int instanceID, int line)
        {
            ProjectileGraphSO graph = EditorUtility.InstanceIDToObject(instanceID) as ProjectileGraphSO;
            if (graph != null)
            {
                OpenWindow();
                SelectGraphToEdit(graph);
                return true;
            }
            return false;
        }
        public static void SelectGraphToEdit(ProjectileGraphSO g)
        {
            ActiveGraph = g;
            if (g != null)
            {
                ActiveGraph.SetActiveGraph();
                if (g != null)
                {
                    Selection.activeObject = g;
                }
            }

        }
        [MenuItem("Projectile Editor", menuItem = "Bremsengine/Projectile Editor")]
        private static void OpenWindow()
        {
            GetWindow<ProjectileGraphEditor>("Projectile Editor");
        }
    }
    #endregion
    #region Grid
    public partial class ProjectileGraphEditor
    {
        const float gridLarge = 100f;
        const float gridSmall = 25f;
        Vector2 graphDrag;

        bool isDraggingGrid;
        private void DragGrid(Vector2 delta)
        {
            graphDrag = delta;
            if (isDraggingGrid && ActiveDrag == null)
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
            DrawGrid(gridSmall, 60, Color.gray);
            DrawGrid(gridLarge, 100, Color.gray);
            DrawGraph();

            if (GUI.changed)
                Repaint();
        }
        void DrawGrid(float gridSize, byte gridOpacity, Color color)
        {
            int vLineCount = Mathf.CeilToInt((position.width + gridSize) / gridSize);
            int hLineCount = Mathf.CeilToInt((position.height + gridSize) / gridSize);
            Handles.color = color.Opacity((byte)gridOpacity);

            Vector2 graphOffset = graphDrag * 0.5f;
            for (int i = 0; i < vLineCount; i++)
            {
                Handles.DrawLine(new Vector2(gridSize * i, -gridSize) + graphOffset, new Vector2(gridSize * i, gridSize + position.height) + graphOffset);
            }
            for (int j = 0; j < hLineCount; j++)
            {
                Handles.DrawLine(new Vector2(-gridSize, gridSize * j) + graphOffset, new Vector2(position.width + gridSize, gridSize * j) + graphOffset);
            }
            Handles.color = Color.white;
        }
        private void DrawGraph()
        {
            ActiveGraph.Draw(projectileNodeStyle);
            GUI.changed = true;
        }
        public static void DrawHeader(Rect rect, string title, float height = 40f)
        {
            float width = rect.width - 25f;
            Rect header = new(new(rect.center.x - (width * 0.5f), rect.center.y + -(rect.height * 0.5f) - height), new(width, height));
            GUILayout.BeginArea(header, title, HeaderStyle);
            GUILayout.EndArea();
        }
    }
    #endregion
    #region Input Events
    public partial class ProjectileGraphEditor
    {
        static ProjectileGraphComponent ActiveDrag;
        static ProjectileGraphComponent LinkAttemptStart;
        static bool IsDragging;
        static Vector2 DragLinePreviewStart;
        #region Mouse Down
        private void ProcessMouseDownEvent(Event e)
        {
            void TryDragGrid(Event e)
            {
                isDraggingGrid = true;
            }
            bool StartDragItem(Event e)
            {
                ForceEndDrag();
                if (e.button == 2)
                {
                    if (IsMouseOverComponent(e, out ProjectileGraphComponent drag))
                    {
                        if (drag != null)
                        {
                            ActiveDrag = drag;
                            IsDragging = true;
                            return true;
                        }
                    }
                    return false;
                }
                if (e.button == 0)
                {
                    ActiveDrag = null;
                    IsDragging = false;
                    if (IsMouseOverComponent(e, out ProjectileGraphComponent linkDrag))
                    {
                        if (linkDrag != null && linkDrag.TryStartLink(out ProjectileGraphComponent linkStart))
                        {
                            Debug.Log("Try Link");
                            LinkAttemptStart = linkStart;
                            ActiveGraph.StartPreviewLine(e.mousePosition);
                            return true;
                        }
                    }
                }
                return false;
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
            IsDragging = false;
            ActiveDrag = null;
            if (e.button == 2)
            {
                isDraggingGrid = false;
            }
            if (e.button == 0)
            {
                if (IsMouseOverComponent(e, out ProjectileGraphComponent hover))
                {
                    if (LinkAttemptStart != null && hover != null && hover != LinkAttemptStart)
                    {
                        LinkAttemptStart.TryCreateLink(hover);
                    }
                }
                ActiveGraph.EndPreviewLine();
                LinkAttemptStart = null;
            }
            /*
            projectileEventDragSelection = null;
            emitterSelection = null;
            directionSelection = null;
            modSelection = null;
            */
        }
        #endregion
        #region Mouse Drag
        public static void ForceEndDrag()
        {
            ActiveDrag = null;
        }
        private void ProcessMouseDrag(Event e)
        {
            if (LinkAttemptStart != null)
            {
                return;
            }
            if (IsDragging && ActiveDrag != null)
            {
                ActiveDrag.Drag(e.delta);
            }
            else
            {
                DragGrid(e.delta);
            }
        }
        #endregion
        #region Process Events
        private void ProcessEvents(Event e)
        {
            graphDrag = Vector2.zero;
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
    }
    #endregion
    #region Projectile Selector Node
    public partial class ProjectileGraphEditor
    {
        private void AddSelector(object mousePosition)
        {
            ProjectileComponentSelector selector = ScriptableObject.CreateInstance<ProjectileComponentSelector>();
            if (selector != null)
            {
                selector.Initialize((Vector2)mousePosition, ActiveGraph);
                AssetDatabase.AddObjectToAsset(selector, ActiveGraph);
                AssetDatabase.SaveAssets();
            }
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
}
#endif