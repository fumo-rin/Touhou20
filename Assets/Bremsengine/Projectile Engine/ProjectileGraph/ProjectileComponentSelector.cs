using UnityEngine;
using UnityEditor;
using Core.Extensions;
namespace Bremsengine
{
#if UNITY_EDITOR
    public partial class ProjectileComponentSelector : ProjectileGraphComponent
    {
        public override string GetGraphComponentName()
        {
            return "Component Selector";
        }

        public override void OnDrag(Vector2 delta)
        {

        }
        public override void OnGraphDelete()
        {
            graph.componentSelectors.Remove(this);
        }

        public override void TryBreakLinks()
        {

        }
        public void DrawContextMenu()
        {
            void AddGraphComponent(ProjectileGraphComponent component, Vector2 position)
            {
                if (component != null)
                {
                    component.Initialize(position, graph);
                    AssetDatabase.AddObjectToAsset(component, graph);
                    AssetDatabase.SaveAssets();
                }
            }
            void CreateComponent(int index)
            {
                Vector2 position = new(rect.min.x, rect.max.y);
                switch (index)
                {
                    case 0: AddGraphComponent(ScriptableObject.CreateInstance<ProjectileEmitterRepeat>(), position); break;
                    case 1: AddGraphComponent(ScriptableObject.CreateInstance<ProjectileEmitterSingle>(), position); break;
                    case 2: AddGraphComponent(ScriptableObject.CreateInstance<ProjectileArcNodeSO>(), position); break;
                    case 3: AddGraphComponent(ScriptableObject.CreateInstance<SingleProjectileNodeSO>(), position); break;
                    case 4: AddGraphComponent(ScriptableObject.CreateInstance<ProjectileGraphDirectionNode>(), position); break;
                    case 5: AddGraphComponent(ScriptableObject.CreateInstance<CrawlerEventSO>(), position); break;
                    case 6: AddGraphComponent(ScriptableObject.CreateInstance<PlaySoundEventSO>(), position); break;
                    case 7: AddGraphComponent(ScriptableObject.CreateInstance<ProjectileModNodeSO>(), position); break;
                    default:
                        break;
                }
            }
            string[] options =
            {
                nameof(ProjectileEmitterRepeat),
                nameof(ProjectileEmitterSingle),
                nameof(ProjectileArcNodeSO),
                nameof(SingleProjectileNodeSO),
                nameof(ProjectileGraphDirectionNode),
                nameof(CrawlerEventSO),
                nameof(PlaySoundEventSO),
                nameof(ProjectileModNodeSO)
            };
            EditorGUI.BeginChangeCheck();
            selected = EditorGUILayout.Popup(selected, options);
            if (GUILayout.Button("Create"))
            {
                CreateComponent(selected);
                graph.DestroyComponent(this);
            }
        }
        protected override Rect GetRect(Vector2 mouse)
        {
            return new Rect(mouse.x, mouse.y, 300f, 100f);
        }
        protected override void OnDraw(GUIStyle style)
        {
            EditorGUI.BeginChangeCheck();
            DrawContextMenu();
            if (EditorGUI.EndChangeCheck())
            {
                this.SetDirtyAndSave();
            }
        }

        protected override void OnInitialize(Vector2 mousePosition, ProjectileGraphSO graph, ProjectileTypeSO type)
        {
            graph.componentSelectors.AddIfDoesntExist(this);
        }

        public override void TryCreateLink(ProjectileGraphComponent other)
        {
            return;
        }

        public override bool TryStartLink(out ProjectileGraphComponent linkStart)
        {
            linkStart = null;
            return false;
        }

        public override bool TryEndLink(out ProjectileGraphComponent linkEnd)
        {
            linkEnd = null;
            return false;
        }

        public override void ReceiveBroadcastUnlink(ProjectileGraphComponent unlink)
        {

        }
        #region Add Components

        #endregion
    }
#endif
    public partial class ProjectileComponentSelector
    {
        int selected = 0;
    }
}
