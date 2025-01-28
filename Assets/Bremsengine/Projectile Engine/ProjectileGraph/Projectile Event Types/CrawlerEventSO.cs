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
            return new(mousePosition.x, mousePosition.y, 400f, 375f);
        }

        protected override void OnDraw(GUIStyle style)
        {
            EditorGUI.BeginChangeCheck();
            AddSpace(1);
            base.OnDraw(style);
            OffScreenClearEdgePadding = (float)EditorGUILayout.IntField("Offscreen Edge Padding Outwards", (int)OffScreenClearEdgePadding);
            isActive = EditorGUILayout.Toggle("Event Enabled", isActive);
            projectilePrefab = (ProjectileTypeSO)EditorGUILayout.ObjectField("Projectile Prefab", projectilePrefab, typeof(ProjectileTypeSO), false);
            fanAngle = EditorGUILayout.Slider("Projectile Fan Angle", fanAngle, 0f, 360f);
            fanProgressionSpeed = EditorGUILayout.CurveField("Arc Progression Speed Modifier", fanProgressionSpeed);
            projectileCopies = EditorGUILayout.IntSlider("Projectile Copies", projectileCopies, 1, 30);
            destroyOriginal = EditorGUILayout.Toggle("Destroy Original", destroyOriginal);
            directionalOffset = EditorGUILayout.Slider("Directional Offset", directionalOffset, 0f, 10f);
            speed = EditorGUILayout.Slider("Projectile Speed", speed, 0f, 35f);
            addedAngle = EditorGUILayout.Slider("Added Angle", addedAngle, -180f, 180f);
            spread = EditorGUILayout.Slider("Spread", spread, 0f, 360f);
            Faction = (BremseFaction)EditorGUILayout.EnumPopup("Faction", Faction);
            Repeats = EditorGUILayout.IntSlider("Repeats", Repeats, 1, 50);
            RepeatInterval = EditorGUILayout.Slider("Time Between Repeats", RepeatInterval, 0.1f, 5f);
            repeatAddedAngle = EditorGUILayout.Slider("Added Angle Per Iteration", repeatAddedAngle, -180f, 180f);

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
        public float OffScreenClearEdgePadding;
        public float repeatAddedAngle = 0f;
        public AnimationCurve fanProgressionSpeed = Helper.InitializedAnimationCurve;
        public int Repeats = 1;
        public float RepeatInterval = 0.5f;
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
        public float CurveValue(AnimationCurve curve, float time)
        {
            if (curve == null || curve.length < 2)
            {
                return 1f;
            }
            return curve.Evaluate(time);
        }
        public float AngleIncrement => fanAngle / (projectileCopies - (fanAngle < 360 ? 1 : 0));
        private IEnumerator CO_Split(float delay, Projectile p, TriggeredEvent e)
        {
            Vector2 ownerVelocity = new();
            yield return new WaitForSeconds(delay);
            float repeatIterationAngle = 0f;
            for (int i = 0; i < Repeats; i++)
            {
                if (p == null || p.transform == null)
                {
                    yield break;
                }
                repeatIterationAngle += repeatAddedAngle;
                float progress = 0f;
                ownerVelocity = p.Velocity;
                float iterationAddedAngle = addedAngle + ((projectileCopies > 1 ? -fanAngle.Multiply(0.5f) : 0f));
                for (int ii = 0; ii < projectileCopies; ii++)
                {
                    //float iterationAddedAngle = ((ii) * fanAngle / (projectileCopies-1).Max(1)) + this.repeatAddedAngle * ii;
                    //ProjectileNodeDirection direction = new ProjectileNodeDirection(p.transform, p.Target, p.Position + p.Velocity.ScaleToMagnitude(1f).Rotate2D(addedAngle).Rotate2D(-fanAngle * 0.5f));
                    #region New
                    ProjectileNodeDirection direction = new ProjectileNodeDirection(p.transform, p.Target, p.Position + p.Velocity.ScaleToMagnitude(1f));
                    float speedMod = CurveValue(fanProgressionSpeed, progress);
                    direction.AddSpeedModifier(speedMod);
                    direction.AddAngle(iterationAddedAngle + repeatIterationAngle);
                    iterationAddedAngle += AngleIncrement;
                    #endregion

                    Projectile spawn = CreateProjectile(projectilePrefab.Prefab, p.Position, direction, directionalOffset, spread, 0f, speed * graph.GetGlobalSpeed());
                    Projectile.RegisterProjectile(spawn);
                    spawn.SetOffScreenClear(OffScreenClearEdgePadding);
                    spawn.SetDamage(p.Damage);
                    spawn.SetFaction(p.Faction);
                    progress = ii == 0 ? 0f : ((float)ii / ((float)projectileCopies - 1)).Clamp(0f, 1f);
                }
                if (destroyOriginal)
                {
                    p.ClearProjectile();
                }
                if (Repeats > 1 && i < Repeats - 1)
                {
                    yield return new WaitForSeconds(RepeatInterval);
                }
            }
        }
    }
}
