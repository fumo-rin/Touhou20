using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Core.Extensions;
using System.Linq;

namespace Bremsengine
{
    #region Editor
#if UNITY_EDITOR
    #region Drag
    public partial class ProjectileNodeSO
    {
        public void DragNode(Vector2 delta)
        {
            rect.position += delta;
            projectileImagePreview.position += delta;
            EditorUtility.SetDirty(this);
            GUI.changed = true;
        }
    }
    #endregion
    #region Events
    public partial class ProjectileNodeSO
    {
        public void ProcessEvents(Event e)
        {
            switch (e.type)
            {
                case EventType.MouseDown:
                    break;
                case EventType.MouseUp:
                    break;
                default:
                    break;
            }
        }
    }
    #endregion
    public partial class ProjectileNodeSO
    {
        [HideInInspector] public Rect rect;
        [HideInInspector] public Rect projectileImagePreview;
        Texture previewTexture;
        public void SetPreviewTexture(ProjectileTypeSO p)
        {
            if (p.Prefab != null && p.Prefab.Texture is Texture t and not null)
            previewTexture = t;
        }
        protected Rect NodeRect(float x, float y, float w, float h)
        {
            return new Rect(new Vector2(x, y), new Vector2(w, h));
        }
        protected Rect NodeRectPreview(float x, float y)
        {
            return new Rect(new Vector2(x - 256, y), new Vector2(250f,250f));
        }
        protected static List<ProjectileTypeSO> ProjectileCache => ProjectileGraphEditor.ProjectileTypeLookup;
        protected abstract void OnInitialize(Rect rect, ProjectileGraphSO graph, ProjectileTypeSO type);
        public void Initialize(Rect rect, ProjectileGraphSO graph, ProjectileTypeSO type)
        {
            this.graph = graph;
            this.name = "Projectile Node";
            this.ID = Guid.NewGuid().ToString();
            this.rect = rect;
            this.projectileImagePreview = NodeRectPreview(rect.x, rect.y+ 30);
            graph.nodes.AddIfDoesntExist(this);
            OnInitialize(rect, graph, type);
        }
        protected abstract void OnDraw(GUIStyle style);
        public void Draw(GUIStyle style)
        {
            GUILayout.BeginArea(rect, style);
            EditorGUI.BeginChangeCheck();

            OnDraw(style);
            GUILayout.EndArea();
            GUILayout.BeginArea(projectileImagePreview, style);
            EditorGUI.DrawPreviewTexture(new(25f,25f,200f,200f), previewTexture);
            if (EditorGUI.EndChangeCheck())
            {
                ProjectileGraphEditor.ForceEndDrag();
                EditorUtility.SetDirty(this);
            }
            GUILayout.EndArea();
        }
        public string[] GetProjectileTypesToDisplay()
        {
            string[] projectileArray = new string[ProjectileCache.Count];

            for (int i = 0; i < ProjectileCache.Count; i++)
            {
                projectileArray[i] = ProjectileCache[i].name;
            }
            return projectileArray;
        }
    }
#endif
    #endregion
    #region Graph Projectile Direction
    public struct ProjectileNodeDirection
    {
        Transform owner;
        Transform target;
        Vector2 overrideDirection;
        Vector2 direction;
        float AngleOffset;
        float Spread;
        float Speed;
        float directionalOffset;
        public ProjectileNodeDirection Clone()
        {
            return new ProjectileNodeDirection()
            {
                owner = this.owner,
                target = this.target,
                AngleOffset = this.AngleOffset,
                overrideDirection = this.overrideDirection,
                direction = this.direction,
                Spread = this.Spread,
                Speed = this.Speed,
            };
        }
        public Vector2 DirectionalOffset => Direction.ScaleToMagnitude(directionalOffset);
        public ProjectileNodeDirection(Transform owner, Transform target, Vector2 overrideDirection)
        {
            this.owner = owner;
            this.target = target;
            this.overrideDirection = overrideDirection;
            this.direction = Vector2.down;
            if (target != null)
            {
                this.direction = target.transform.position - owner.transform.position;
            }
            if (overrideDirection != Vector2.zero)
            {
                this.direction = overrideDirection;
            }
            this.Spread = 0f;
            this.AngleOffset = 0f;
            this.Speed = 0f;
            this.directionalOffset = 0.25f;
        }
        public ProjectileNodeDirection SetSpeed(float speed)
        {
            Speed = speed;
            return this;
        }
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
        private Vector2 RotatedDirection => direction.Rotate2D(AngleOffset).Rotate2D(Spread);
        public Vector2 Direction => RotatedDirection.ScaleToMagnitude(Speed);
    }
    #endregion
    public abstract partial class ProjectileNodeSO : ScriptableObject
    {
        public string ID;
        public float spawnDelay;
        List<object> linkedProjectileEvents = new();
        public abstract List<Projectile> Spawn(Transform owner, Transform target, Vector2 lastTargetPosition);

        [HideInInspector] public ProjectileGraphSO graph;
        protected void SendProjectileEvents(List<Projectile> projectiles)
        {
            foreach (Projectile projectile in projectiles.Where(x => x is not null and Projectile))
            {
                
            }
        }
        protected Projectile CreateProjectile(Projectile p, Vector2 position, ProjectileNodeDirection direction)
        {
            return Projectile.NewCreateFromQueue(p, position, direction);
        }
    }
}
