using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Core.Extensions;

namespace Bremsengine
{
#if UNITY_EDITOR
    public partial class ProjectileArcNodeSO
    {
        public override string GetGraphComponentName()
        {
            return "Projectile Arc";
        }
        public override void OnDrag(Vector2 delta)
        {
            base.OnDrag(delta);
        }

        public override void OnGraphDelete()
        {

        }

        protected override Rect GetRect(Vector2 mousePosition)
        {
            return new Rect(mousePosition.x, mousePosition.y, 350f, 700f);
        }
        protected override void OnDraw(GUIStyle style)
        {
            base.OnDraw(style);
            DrawLabel("Projectile Arc Settings");
            projectileCountFloat = EditorGUILayout.Slider("Projectile Count", projectileCountFloat.Floor(), 1f, 60f);
            AngleCoverage = EditorGUILayout.Slider("Angle Coverage", AngleCoverage, 0, 360f);
            startingAngle = EditorGUILayout.Slider("Starting Angle", startingAngle, -180, 180f);
            RandomAngle = EditorGUILayout.Toggle("Random Angle", RandomAngle);
            arcProgressionSpeed = EditorGUILayout.CurveField("Arc Progression Speed Modifier", arcProgressionSpeed);
            arcProgressionAngleMultiplier = EditorGUILayout.CurveField("Arc Progression Angle Modifier", arcProgressionAngleMultiplier);
        }
    }
#endif
    public partial class ProjectileArcNodeSO : ProjectileNodeSO
    {
        public float projectileCountFloat = 7f;
        private int ProjectileCount => projectileCountFloat.ToInt();
        public float AngleCoverage = 60f;
        public float startingAngle = 0f;
        public bool RandomAngle = false;
        public AnimationCurve arcProgressionSpeed = Helper.InitializedAnimationCurve;
        public AnimationCurve arcProgressionAngleMultiplier = Helper.InitializedAnimationCurve;
        public float CurveValue(AnimationCurve curve, float time)
        {
            if (curve == null || curve.length < 2)
            {
                return 1f;
            }
            return curve.Evaluate(time);
        }
        public float AngleIncrement => AngleCoverage / (ProjectileCount - (AngleCoverage < 360 ? 1 : 0));
        public override void Spawn(in List<Projectile> l, Transform owner, Transform target, Vector2 lastTargetPosition, TriggeredEvent triggeredEvent)
        {
            float progress = 0f;
            int iteration = 0;
            float iterationAngle = (startingAngle + (ProjectileCount > 1 ? -AngleCoverage.Multiply(0.5f) : 0f));
            for (int i = 0; i < ProjectileCount; i++)
            {
                ProjectileNodeDirection direction = BuildDirection(owner, target);
                direction.AddSpeedModifier(CurveValue(arcProgressionSpeed, progress));

                direction.AddAngle(RandomAngle ? Random.Range(0f, AngleCoverage) : iterationAngle.Multiply(CurveValue(arcProgressionAngleMultiplier, progress)));

                Projectile spawn = CreateProjectile(ProjectileType.Prefab, owner.position, direction);
                l.Add(spawn);
                iterationAngle += AngleIncrement;
                iteration++;
                progress = iteration == 0 ? 0f : ((float)iteration / ((float)ProjectileCount - 1)).Clamp(0f, 1f);
                SendProjectileEvents(spawn, triggeredEvent);
            }
        }
    }
}
