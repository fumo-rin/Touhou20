using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Core.Extensions;

namespace Bremsengine
{
    #region Editor
#if UNITY_EDITOR
    using UnityEditor;
    public partial class ProjectileEmitterSO : ProjectileGraphComponent
    {
        public override string GetGraphComponentName()
        {
            return "Projectile Emitter";
        }

        public override void OnDrag(Vector2 delta)
        {

        }

        public override void OnGraphDelete()
        {
            graph.emitters.Remove(this);
        }

        protected override Rect GetRect(Vector2 mousePosition)
        {
            return new(mousePosition, new(350f, 200f));
        }

        protected override void OnDraw(GUIStyle style)
        {
            EditorGUI.BeginChangeCheck();
            addedDelay = EditorGUILayout.Slider("Added Delay", addedDelay, 0f, 3f);
            OffScreenClearEdgePadding = (float)EditorGUILayout.IntField("Offscreen Edge Padding Outwards", (int)OffScreenClearEdgePadding);
            Retargetting = EditorGUILayout.Toggle("Retargetting", Retargetting);

            Active = EditorGUILayout.Toggle("Is Active", Active);
            if (EditorGUI.EndChangeCheck())
            {
                EditorUtility.SetDirty(this);
                AssetDatabase.SaveAssetIfDirty(this);
            }
        }

        protected override void OnInitialize(Vector2 mousePosition, ProjectileGraphSO graph, ProjectileTypeSO type)
        {
            this.graph = graph;
            this.name = "Projectile Node";
            if (string.IsNullOrEmpty(ID))
            {
                this.ID = Guid.NewGuid().ToString();
            }
            this.rect = GetRect(mousePosition);
        }
        public void BreakLinks()
        {
            for (int i = 0; i < linkedNodes.Count; i++)
            {
                if (linkedNodes[i] != null)
                {
                    linkedNodes.RemoveAt(i);
                    i--;
                }
            }
            LinkDirection(null);
        }
    }
#endif
    #endregion
    public abstract partial class ProjectileEmitterSO : ProjectileGraphComponent
    {
        public bool Active = true;
        public List<ProjectileNodeSO> linkedNodes = new List<ProjectileNodeSO>();
        public float addedDelay;
        public bool Retargetting;
        public float CooldownDuration => GetCooldownDelay();
        public float OffScreenClearEdgePadding;
        protected abstract float GetCooldownDelay(); 
        public ProjectileGraphDirectionNode linkedOverrideDirection;
        public abstract void Trigger(TriggeredEvent triggeredEvent, ProjectileGraphInput input, Projectile.SpawnCallback callback);
        public void LinkDirection(ProjectileGraphDirectionNode direction)
        {
            if (direction == null)
            {
                linkedOverrideDirection = null;
                return;
            }
            linkedOverrideDirection = direction;
        }
        protected IEnumerator Co_Emit(float delay, TriggeredEvent triggeredEvent, ProjectileGraphInput input, Projectile.SpawnCallback callback)
        {
            yield return new WaitForSeconds(delay);
            triggeredEvent.ClearPlayedSounds();
            List<Projectile> newSpawns = new();

            Vector2 aimDirection = Vector2.zero;
            if (linkedOverrideDirection != null)
            {
                aimDirection = linkedOverrideDirection.GetDirection();
                input.SetOverrideDirection(aimDirection);
            }
            else if (!Retargetting && input.Target && input.TargetStartPosition != null)
            {
                aimDirection = ((Vector2)input.TargetStartPosition - input.Position);
                input.SetOverrideDirection(aimDirection);
            }
            else if (input.Target)
            {
                aimDirection = ((Vector2)input.Target.position - input.Position);
                input.SetOverrideDirection(aimDirection);
            }
            foreach (var item in linkedNodes)
            {
                item.Spawn(in newSpawns, input, triggeredEvent);
            }
            foreach (var item in newSpawns)
            {
                callback?.Invoke(item, input.Owner, input.Target);
                item.SetOffScreenClear(OffScreenClearEdgePadding);
                Projectile.RegisterProjectile(item);
            }
        }
    }
}
