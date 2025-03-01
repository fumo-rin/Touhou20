using UnityEditor;
using UnityEngine;

namespace Bremsengine
{
    #region Editor
#if UNITY_EDITOR
    public partial class ProjectileGraphDirectionNode
    {
        public void BreakDirectionLinks()
        {
            foreach (var item in graph.components)
            {
                if (item is ProjectileEmitterSO e && e.linkedOverrideDirection == this)
                {
                    e.LinkDirection(null);
                }
            }
        }
        public override string GetGraphComponentName()
        {
            return "Direction Override : " + overrideDirection.ToString("F1");
        }

        public override void OnDrag(Vector2 delta)
        {

        }

        public override void OnGraphDelete()
        {
            foreach (var item in graph.components)
            {
                if (item is ProjectileEmitterSO emitter)
                {
                    if (emitter.linkedOverrideDirection == this)
                    {
                        emitter.LinkDirection(null);
                    }
                }
            }
        }

        public override void ReceiveBroadcastUnlink(ProjectileGraphComponent unlink)
        {

        }

        public override void TryBreakLinks()
        {
            foreach(var item in graph.components)
            {
                if (item is ProjectileEmitterSO emitter && emitter.linkedOverrideDirection == this)
                {
                    emitter.linkedOverrideDirection = null;
                }
            }
        }

        public override void TryCreateLink(ProjectileGraphComponent other)
        {
            if (other is ProjectileEmitterSO emitter)
            {
                emitter.linkedOverrideDirection = this;
            }
        }

        protected override Rect GetRect(Vector2 mousePosition)
        {
            return new(new(mousePosition.x,mousePosition.y), new(250f,200f));
        }

        protected override void OnDraw(GUIStyle style)
        {
            EditorGUI.BeginChangeCheck();
            overrideDirection = EditorGUILayout.Vector2Field("Override Direction", overrideDirection);
            if (EditorGUI.EndChangeCheck())
            {
                EditorUtility.SetDirty(this);
                AssetDatabase.SaveAssetIfDirty(this);
            }
        }

        protected override void OnInitialize(Vector2 mousePosition, ProjectileGraphSO graph, ProjectileTypeSO type)
        {

        }
    }
#endif
    #endregion
    public partial class ProjectileGraphDirectionNode : ProjectileGraphComponent
    {
        public Vector2 overrideDirection = new(0f, -1f);
        public Vector2 GetDirection() => overrideDirection;
    }
}
