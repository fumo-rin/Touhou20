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
        protected override void OnDraw(GUIStyle style)
        {
            DrawLabel("Projectile Arc Settings");
            projectileCountFloat = EditorGUILayout.Slider("Projectile Count", projectileCountFloat.Floor(), 1f, 60f);
            AngleCoverage = EditorGUILayout.Slider("Angle Coverage", AngleCoverage, 0, 360f);
            startingAngle = EditorGUILayout.Slider("Starting Angle", startingAngle, -180, 180f);
            RandomAngle = EditorGUILayout.Toggle("Random Angle", RandomAngle);
        }

        protected override void OnInitialize(Rect rect, ProjectileGraphSO graph, ProjectileTypeSO type)
        {

        }
    }
#endif
    public partial class ProjectileArcNodeSO : ProjectileNodeSO
    {
        private float projectileCountFloat = 7f;
        private int ProjectileCount => projectileCountFloat.ToInt();
        private float AngleCoverage;
        private float startingAngle;
        private bool RandomAngle;
        public float AngleIncrement => AngleCoverage / (ProjectileCount - (AngleCoverage < 360 ? 1 : 0));
        public override void Spawn(in List<Projectile> l, Transform owner, Transform target, Vector2 lastTargetPosition)
        {
            float progress = 0f;
            int iteration = 0;
            float iterationAngle = (startingAngle + (ProjectileCount > 1 ? -AngleCoverage.Multiply(0.5f) : 0f));
            for (int i = 0; i < ProjectileCount; i++)
            {
                ProjectileNodeDirection direction = BuildDirection(owner, target);

                direction.AddAngle(RandomAngle ? Random.Range(0f,AngleCoverage) : iterationAngle);

                l.Add(CreateProjectile(ProjectileType.Prefab, owner.position, direction));
                iterationAngle += AngleIncrement;
                iteration++;
                progress = iteration == 0 ? 0f : ((float)iteration / ((float)ProjectileCount - 1)).Clamp(0f, 1f);
            }
        }
    }
}
