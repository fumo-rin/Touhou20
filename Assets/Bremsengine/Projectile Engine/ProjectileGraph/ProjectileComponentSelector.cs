using UnityEngine;
using UnityEditor;
using Core.Extensions;
using System;
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
            void AddGraphComponent<T>(Vector2 position) where T : ProjectileGraphComponent
            {
                if (ScriptableObject.CreateInstance<T>() is ProjectileGraphComponent spawned and not null)
                {
                    spawned.Initialize(position, graph);
                    AssetDatabase.AddObjectToAsset(spawned, graph);
                    graph.Dirty();
                }
            }
            void CreateComponent(int index)
            {
                Vector2 position = new(rect.min.x, rect.max.y);
                switch (index)
                {
                    case 0: AddGraphComponent<ProjectileEmitterRepeat>(position); break;
                    case 1: AddGraphComponent<ProjectileEmitterSingle>(position); break;
                    case 2: AddGraphComponent<ProjectileArcNodeSO>(position); break;
                    case 3: AddGraphComponent<SingleProjectileNodeSO>(position); break;
                    case 4: AddGraphComponent<ProjectileGraphDirectionNode>(position); break;
                    case 5: AddGraphComponent<CrawlerEventSO>(position); break;
                    case 6: AddGraphComponent<PlaySoundEventSO>(position); break;
                    case 7: AddGraphComponent<ProjectileModNodeSO>(position); break;
                    case 8: AddGraphComponent<ProjectileRotateArcNodeSO>(position); break;
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
                nameof(ProjectileModNodeSO),
                nameof(ProjectileRotateArcNodeSO)
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
