using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using UnityEditor;
using Core.Extensions;

namespace Bremsengine
{
#if UNITY_EDITOR
    public partial class ProjectileModNodeSO
    {
        public override string GetGraphComponentName()
        {
            return "Projectile Mod Container";
        }

        public override void OnDrag(Vector2 delta)
        {

        }

        public override void OnGraphDelete()
        {

        }

        protected override Rect GetRect(Vector2 mousePosition)
        {
            return new Rect(mousePosition.x, mousePosition.y, 350f,150f);
        }

        protected override void OnDraw(GUIStyle style)
        {
            containedMod = (ProjectileMod)EditorGUILayout.ObjectField("Contained Mod", containedMod, typeof(ProjectileMod), false);
            isEnabled = EditorGUILayout.Toggle("Is Enabled", isEnabled);
        }
        protected override void OnInitialize(Vector2 mousePosition, ProjectileGraphSO graph, ProjectileTypeSO type)
        {
            graph.modNodes.AddIfDoesntExist(this);
        }

        protected override void SecondaryDraw(GUIStyle style)
        {

        }
        public void BreakLinks()
        {
            attachedNodes.Clear();
        }

        public override void TryBreakLinks()
        {
            attachedNodes.Clear();
        }

        public override void TryCreateLink(ProjectileGraphComponent other)
        {
            if (other is ProjectileNodeSO node)
            {
                attachedNodes.AddIfDoesntExist(node);
            }
            if (other is ProjectileEmitterSO emitter)
            {
                foreach (var item in emitter.linkedNodes)
                {
                    attachedNodes.Add(item);
                }
            }
        }

        public override void ReceiveBroadcastUnlink(ProjectileGraphComponent unlink)
        {
            attachedNodes.Remove(unlink as ProjectileNodeSO);
        }
    }
#endif
    public partial class ProjectileModNodeSO : ProjectileGraphComponent
    {
        public List<ProjectileNodeSO> attachedNodes = new();
        public ProjectileMod containedMod;
        public bool isEnabled = true;
    }
}
