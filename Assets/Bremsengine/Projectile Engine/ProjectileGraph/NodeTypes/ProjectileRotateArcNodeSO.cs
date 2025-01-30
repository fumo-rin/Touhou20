using System.Collections.Generic;
using UnityEngine;

namespace Bremsengine
{
    using Core.Extensions;
    #region Editor
#if UNITY_EDITOR
    using UnityEditor;

    public partial class ProjectileRotateArcNodeSO
    {
        public override string GetGraphComponentName()
        {
            return "Projectile Rotate Arc";
        }
        protected override Rect GetRect(Vector2 mousePosition)
        {
            return new(mousePosition, new(350f, 500f));
        }

        protected override void OnDraw(GUIStyle style)
        {
            base.OnDraw(style);
            angleStepCurve = EditorGUILayout.CurveField("Angle Steps", angleStepCurve);
            speedModCurve = EditorGUILayout.CurveField("Speedmod Steps", speedModCurve);
            stepSize = EditorGUILayout.Slider("Step Size", stepSize, 0.03f, 1f);
            maximumStepCount = EditorGUILayout.IntSlider("Maximum Step Count", maximumStepCount, 1, 100);
        }
    }
#endif
    #endregion
    public partial class ProjectileRotateArcNodeSO : ProjectileNodeSO
    {
        public AnimationCurve angleStepCurve = Helper.InitializedAnimationCurve;
        public AnimationCurve speedModCurve = Helper.InitializedAnimationCurve;
        public float stepSize = 0.1f;
        public int maximumStepCount = 100;
        public override void Spawn(in List<Projectile> newProjectiles, ProjectileGraphInput input, TriggeredEvent triggeredEvent)
        {
            foreach (float i in stepSize.StepFromTo(0f, angleStepCurve.Duration().Min(speedModCurve.Duration()).Absolute()))
            {
                float curveAngle = angleStepCurve.Evaluate(i);
                float speedMod = speedModCurve.Evaluate(i);
                ProjectileNodeDirection direction = BuildDirectionAlternate(input);
                direction.ReverseSpeed = ReverseDirection;

                direction.AddAngle(input.addedAngle + curveAngle);
                direction.AddSpeedModifier(speedMod);
                Projectile spawn = CreateProjectile(ProjectileType.Prefab, input.OwnerCurrentPosition, direction);
                newProjectiles.Add(spawn);
                SendProjectileEvents(spawn, triggeredEvent);
                RunModsForProjectile(spawn);
            }
        }
    }
}
