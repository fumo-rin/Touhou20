using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Core;
using System.Linq;





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
        public static List<ProjectileTypeSO> Cache => LoadCache();
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
            return projectilePrefabs;
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
        private void ProcessEvents(Event e)
        {
            ProcessProjectileGraphEvents(e);
        }
        private void ProcessProjectileGraphEvents(Event e)
        {
            switch (e.type)
            {
                case EventType.MouseDown:
                    ProcessMouseDownEvent(e);
                    break;
                default:
                    break;
            }
        }
        #region Mouse Down
        private void ProcessMouseDownEvent(Event e)
        {
            if (e.button == 1)
            {
                ShowContextMenu(e.mousePosition);
            }
        }
        private void ShowContextMenu(Vector2 position)
        {
            GenericMenu menu = new GenericMenu();
            menu.AddItem(new GUIContent("Create Projectile Node"), false, CreateProjectileNode, position);
            menu.ShowAsContext();
        }
        #endregion
    }
    #endregion
    #region Active Nodes
    public partial class ProjectileGraphEditor
    {
        private void CreateProjectileNode(object mousePositionObject)
        {
            LoadCache();
            if (projectilePrefabs == null || projectilePrefabs.Count <= 0)
            {
                Debug.LogWarning("projectiles arent initialized properly or doesnt exist");
                return;
            }
            CreateProjectileNode(mousePositionObject, projectilePrefabs[0]);
        }
        private void CreateProjectileNode(object mousePositionObject, ProjectileTypeSO prefab)
        {
            Vector2 mousePosition = (Vector2)mousePositionObject;
            ProjectileNodeSO newNode = ScriptableObject.CreateInstance<ProjectileNodeSO>();
            newNode.Initialize(NodeRect(mousePosition.x, mousePosition.y), ActiveGraph, prefab);
            
            AssetDatabase.AddObjectToAsset(newNode, ActiveGraph);
            AssetDatabase.SaveAssets();
        }
    }
    #endregion
    public partial class ProjectileGraphEditor : UnityEditor.EditorWindow
    {
        private GUIStyle projectileNodeStyle;

        private const float nodeWidth = 160f;
        private const float nodeHeight = 75f;
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
        Rect NodeRect(float x, float y)
        {
            return new Rect(new Vector2(x, y), new Vector2(nodeWidth, nodeHeight));
        }
    }
}
#endif