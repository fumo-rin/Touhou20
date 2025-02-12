using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Core.Extensions;
using System.Linq;
using System.IO;
using System.Runtime.InteropServices;

namespace Bremsengine
{
    #region Editor
#if UNITY_EDITOR
    public partial class ProjectileNodeSO : ProjectileGraphComponent
    {
        const float spritePreviewSize = 250f;
        [HideInInspector] public Rect projectileImagePreview;
        Texture previewTexture;
        public void SetPreviewTexture(ProjectileTypeSO p)
        {
            if (p.Prefab != null && p.Prefab.Texture is Texture t and not null)
                previewTexture = t;
        }
        protected Rect NodeRectPreview(float x, float y)
        {
            return new Rect(new Vector2(x - 256, y), new Vector2(250f, 250f));
        }
        protected override void OnInitialize(Vector2 position, ProjectileGraphSO graph, ProjectileTypeSO type)
        {
            this.graph = graph;
            this.name = "Projectile Node";
            if (string.IsNullOrEmpty(ID))
            {
                this.ID = Guid.NewGuid().ToString();
            }
            this.rect = GetRect(position);
            this.projectileImagePreview = NodeRectPreview(position.x, position.y + 30);
            graph.nodes.AddIfDoesntExist(this);
            this.ProjectileType = type;
        }
        protected override void OnDraw(GUIStyle style)
        {
            EditorGUI.BeginChangeCheck();
            ProjectileType = (ProjectileTypeSO)EditorGUILayout.ObjectField("Projectile Type", ProjectileType, typeof(ProjectileTypeSO), false);
            directionalOffset = EditorGUILayout.Slider("Directional Offset", directionalOffset, 0f, 25f);
            spread = EditorGUILayout.Slider("Spread", spread, 0f, 60f);
            speed = EditorGUILayout.Slider("Speed", speed, 0f, 35f);
            addedAngle = EditorGUILayout.Slider("Added Angle", addedAngle, -180f, 180f);
            ReverseDirection = EditorGUILayout.Toggle("Reverse Speed", ReverseDirection);
            FlareIndex = EditorGUILayout.IntField("Flare ID", FlareIndex);

            if (EditorGUI.EndChangeCheck())
            {
                EditorUtility.SetDirty(this);
                AssetDatabase.SaveAssetIfDirty(this);
            }
        }
        protected override void SecondaryDraw(GUIStyle style)
        {

        }
        public override void OnDrag(Vector2 delta)
        {
            projectileImagePreview.position += delta;
        }
        public override void OnGraphDelete()
        {
            foreach (var item in graph.emitters)
            {
                item.linkedNodes.Remove(this);
            }
            foreach (var item in graph.components)
            {
                if (item is ProjectileModNodeSO mod)
                {
                    mod.attachedNodes.Remove(this);
                }
                if (item is ProjectileEmitterSO emitter)
                {
                    emitter.linkedNodes.Remove(this);
                }
            }
            graph.nodes.Remove(this);
        }

        public override void TryBreakLinks()
        {
            foreach (var c in graph.components)
            {
                if (c is ProjectileModNodeSO mod)
                {
                    mod.attachedNodes.Remove(this);
                }
            }
        }
        public override void TryCreateLink(ProjectileGraphComponent other)
        {
            if (other is ProjectileModNodeSO mod)
            {
                mod.attachedNodes.AddIfDoesntExist(this);
            }
            if (other is ProjectileEmitterSO emitter)
            {
                emitter.linkedNodes.AddIfDoesntExist(this);
            }
            if (other is ProjectileEventSO eventSo)
            {
                linkedProjectileEvents.AddIfDoesntExist(eventSo);
            }
        }

        public override void ReceiveBroadcastUnlink(ProjectileGraphComponent unlink)
        {
            linkedProjectileEvents.Remove(unlink as ProjectileEventSO);
            if (unlink is ProjectileModNodeSO modNode)
            {
                linkedProjectileMods.Remove(modNode.containedMod);
            }
        }
    }
#endif
    #endregion
    #region Graph Projectile Direction
    public struct ProjectileNodeDirection
    {
        public Transform owner { get; private set; }
        public Transform target { get; private set; }
        Vector2 direction;
        float AngleOffset;
        float Spread;
        float Speed;
        float directionalOffset;
        float speedMod;
        public int flareIndex { get; private set; }
        public bool ReverseSpeed;
        public ProjectileNodeDirection Clone()
        {
            return new ProjectileNodeDirection()
            {
                owner = this.owner,
                target = this.target,
                AngleOffset = this.AngleOffset,
                direction = this.direction,
                Spread = this.Spread,
                Speed = this.Speed,
                speedMod = this.speedMod,
                directionalOffset = this.directionalOffset,
                ReverseSpeed = this.ReverseSpeed
            };
        }
        public ProjectileNodeDirection SetManualDirection(Vector2 direction)
        {
            this.direction = direction;
            return this;
        }
        public Vector2 DirectionalOffset => Direction.ScaleToMagnitude(directionalOffset);
        public ProjectileNodeDirection(Transform owner, Transform target, Vector2 overrideTargetPosition)
        {
            this.owner = owner;
            this.target = target;
            this.direction = overrideTargetPosition != Vector2.zero ? overrideTargetPosition - (Vector2)owner.position : (Vector2)owner.position + Vector2.right;
            if (target != null && overrideTargetPosition == Vector2.zero)
            {
                this.direction = target.position - owner.position;
            }
            this.Spread = 0f;
            this.AngleOffset = 0f;
            this.Speed = 0f;
            this.directionalOffset = 0.25f;
            this.speedMod = 1f;
            this.ReverseSpeed = false;
            this.flareIndex = 0;
        }
        public ProjectileNodeDirection SetFlare(int i)
        {
            this.flareIndex = i;
            return this;
        }
        public ProjectileNodeDirection SetSpeed(float speed)
        {
            Speed = speed * speedMod;
            return this;
        }
        public ProjectileNodeDirection AddSpeedModifier(float speedMod)
        {
            this.speedMod *= speedMod;
            return this;
        }
        public float SpeedMod => speedMod;
        public ProjectileNodeDirection AddAngle(float angle)
        {
            AngleOffset += angle;
            return this;
        }
        public ProjectileNodeDirection SetSpread(float spread)
        {
            Spread = spread;
            return this;
        }
        public ProjectileNodeDirection SetDirectionalOffset(float offset)
        {
            directionalOffset = offset;
            return this;
        }
        private Vector2 RotatedDirection => direction.Rotate2D(AngleOffset).Rotate2D(Spread.RandomPositiveNegativeRange());
        public Vector2 Direction => RotatedDirection.ScaleToMagnitude(Speed);
        public Vector2 VelocityDirection => Direction.ScaleToMagnitude(Speed) * (ReverseSpeed ? -1f : 1f);
    }
    #endregion
    #region Direction
    public partial class ProjectileNodeSO
    {
        public float directionalOffset = 0f;
        public float spread = 0f;
        public float speed = 10f;
        public float addedAngle = 0f;
        public bool ReverseDirection = false;
        public float ReverseDirectionModifier => ReverseDirection ? -1f : 1f;
        public ProjectileNodeDirection BuildDirection(Transform owner, Transform target, Vector2 overrideTarget)
        {
            Vector2 o = overrideTarget != Vector2.zero ? overrideTarget : target.position;
            ProjectileNodeDirection direction = new(owner, target, o);
            return direction;
        }
        public ProjectileNodeDirection BuildDirectionAlternate(ProjectileGraphInput input)
        {
            Vector2 o = input.AimTargetPosition;
            ProjectileNodeDirection d = new(input.Owner, input.Target, o);
            return d;
        }
    }
    #endregion
    public abstract partial class ProjectileNodeSO : ProjectileGraphComponent
    {
        public ProjectileTypeSO ProjectileType;
        public float spawnDelay;
        public int FlareIndex = 0;
        public List<ProjectileEventSO> linkedProjectileEvents = new();
        public List<ProjectileMod> linkedProjectileMods = new();
        public abstract void Spawn(in List<Projectile> list, ProjectileGraphInput input, TriggeredEvent triggeredEvent);
        public void SendProjectileEvents(Projectile p, TriggeredEvent triggeredEvent)
        {
            triggeredEvent.Bind(this);
            foreach (var linkedEvent in linkedProjectileEvents)
            {
                if (triggeredEvent.HasPlayedEvent(p, linkedEvent))
                    continue;
                SendProjectileEvents(p, linkedEvent, triggeredEvent);
            }
        }
        private void SendProjectileEvents(Projectile p, ProjectileEventSO projectileEvent, TriggeredEvent triggeredEvent)
        {
            if (p == null)
            {
                return;
            }
            foreach (ProjectileEventSO e in linkedProjectileEvents)
            {
                e.QueueEvents(p, this, triggeredEvent);
            }
        }
        protected void RunModsForProjectile(Projectile p)
        {
            foreach (var item in graph.modNodes)
            {
                if (!item.isEnabled || !item.attachedNodes.Contains(this))
                    continue;
                WaitForSeconds delay = item.containedMod.Delay;
                item.containedMod.QueueMod(p, delay);
            }
        }
        protected Projectile CreateProjectile(Projectile p, Vector2 position, ProjectileNodeDirection direction)
        {
            if (p == null)
            {
                Debug.Log("Missing Prefab");
                return null;
            }
            direction.SetSpeed(speed * graph.GetGlobalSpeed());
            direction.AddAngle(addedAngle);
            direction.SetSpread(spread);
            direction.SetDirectionalOffset(directionalOffset);
            direction.SetFlare(FlareIndex);
            Projectile spawnProjectile = Projectile.NewCreateFromQueue(p, position, direction);
            return spawnProjectile;
        }
    }
}
