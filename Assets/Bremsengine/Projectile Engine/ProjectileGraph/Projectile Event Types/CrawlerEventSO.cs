using Core.Extensions;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Bremsengine
{
    #region Editor
#if UNITY_EDITOR
    public partial class CrawlerEventSO
    {
        public override string GetGraphComponentName()
        {
            return "Projectile Crawler Event";
        }

        public override void OnDrag(Vector2 delta)
        {

        }

        protected override Rect GetRect(Vector2 mousePosition)
        {
            return new(mousePosition.x, mousePosition.y, 400f, 300f);
        }

        protected override void OnDraw(GUIStyle style)
        {
            EditorGUI.BeginChangeCheck();
            AddSpace(1);
            base.OnDraw(style);
            isActive = EditorGUILayout.Toggle("Event Enabled", isActive);
            projectilePrefab = (ProjectileTypeSO)EditorGUILayout.ObjectField("Projectile Prefab", projectilePrefab, typeof(ProjectileTypeSO), false);
            fanAngle = EditorGUILayout.Slider("Projectile Fan Angle", fanAngle, 0f, 360f);
            projectileCopies = EditorGUILayout.IntSlider("Projectile Copies", projectileCopies, 1, 30);
            destroyOriginal = EditorGUILayout.Toggle("Destroy Original", destroyOriginal);
            directionalOffset = EditorGUILayout.Slider("Directional Offset", directionalOffset, 0f, 10f);
            speed = EditorGUILayout.Slider("Projectile Speed", speed, 0f, 35f);
            addedAngle = EditorGUILayout.Slider("Added Angle", addedAngle, -180f, 180f);
            spread = EditorGUILayout.Slider("Spread", spread, 0f, 360f);
            Faction = (BremseFaction)EditorGUILayout.EnumPopup("Faction", Faction);

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
    public partial class CrawlerEventSO : ProjectileEventSO
    {
        public bool isActive = true;
        public ProjectileTypeSO projectilePrefab;
        public float fanAngle = 20f;
        public int projectileCopies = 3;
        public bool destroyOriginal;
        public float directionalOffset = 0.25f;
        public float speed = 4f;
        public float addedAngle = 0f;
        public float spread = 0f;
        public BremseFaction Faction;

        protected override void TriggerEvent(Projectile p, TriggeredEvent e)
        {
            if (!isActive)
                return;

            if (e.HasPlayedEvent(p, this))
                return;
            e.RegisterEvent(p, this);

            p.StartCoroutine(CO_Split(EventDelay, p, e));
        }
        private IEnumerator CO_Split(float delay, Projectile p, TriggeredEvent e)
        {
            Vector2 ownerVelocity = p.Velocity;
            yield return new WaitForSeconds(delay);
            if (p != null && p.transform != null)
            {
                for (int i = 0; i < projectileCopies; i++)
                {
                    float iterationAddedAngle = i * fanAngle / (projectileCopies -1).Max(1);
                    ProjectileNodeDirection direction = new ProjectileNodeDirection(p.transform, p.Target, p.Position + p.Velocity.ScaleToMagnitude(1f).Rotate2D(addedAngle).Rotate2D(-fanAngle*0.5f));

                    Projectile spawn = CreateProjectile(projectilePrefab.Prefab, p.Position, direction, directionalOffset, spread, iterationAddedAngle, speed);
                    Projectile.RegisterProjectile(spawn);
                    spawn.SetDamage(p.Damage);
                    spawn.SetFaction(p.Faction);
                }
                if (destroyOriginal)
                {
                    p.ClearProjectile();
                }
            }
        }
    }
}
