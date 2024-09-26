using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using System;
using System.ComponentModel;
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
            Retargetting = EditorGUILayout.Toggle("Retargetting", Retargetting);

            Active = EditorGUILayout.Toggle("Is Active",Active);
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
        public abstract void Trigger(TriggeredEvent triggeredEvent, ProjectileGraphInput input, Projectile.SpawnCallback callback);
        protected IEnumerator Co_Emit(float delay, TriggeredEvent triggeredEvent, ProjectileGraphInput input, Projectile.SpawnCallback callback)
        {
            yield return new WaitForSeconds(delay);
            triggeredEvent.ClearPlayedSounds();
            List<Projectile> newSpawns = new();

            foreach (var item in linkedNodes)
            {
                item.Spawn(in newSpawns, input, triggeredEvent);
            }
            foreach (var item in newSpawns)
            {
                callback?.Invoke(item, input.Owner, input.Target);
                Projectile.RegisterProjectile(item);
            }
        }
    }
}
