using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Bremsengine;
using Core.Extensions;

namespace BremseTouhou
{
    [CreateAssetMenu(menuName = "Bremse Touhou/Composite Attacks/Projectile Arc")]
    public class ProjectileArc : UnitAttack
    {
        public List<ProjectileArcEntry> arc = new();
        public override void AttackTarget(BaseUnit owner, Vector2 origin, Vector2 target)
        {
            ProjectileDirection directionIteration;
            float progress;
            int iteration; 
            float iterationAngle;
            foreach (ProjectileArcEntry entry in arc)
            {
                progress = 0f;
                iteration = 0;
                iterationAngle = entry.startingAngle + (entry.ProjectileCount > 1 ? -entry.AngleCoverage.Multiply(0.5f) : 0f);
                for (int i = 0; i < entry.ProjectileCount; i++)
                {
                    directionIteration = new ProjectileDirection(entry.Projectile, target - origin)
                        .AddAngle(iterationAngle);

                    directionIteration.SetSpeedMod(entry.GetSpeed(progress));

                    SpawnProjectile(entry.Projectile, owner, origin, directionIteration);

                    iterationAngle += entry.AngleIncrement;
                    iteration++;
                    progress = iteration == 0 ? 0f : ((float)iteration / ((float)entry.ProjectileCount -1)).Clamp(0f,1f);
                }
            }
        }
        private Projectile SpawnProjectile(ProjectileSO projectile, BaseUnit owner, Vector2 origin, ProjectileDirection d)
        {
            Projectile p = Projectile.SpawnProjectile(projectile, origin, d, OnProjectileSpawn);
            p.SetFaction(owner == null ? BremseFaction.None : owner.FactionInterface.Faction);
            return p;
        }
    }
    [System.Serializable]
    public class ProjectileArcEntry
    {
        public float startingAngle = 0f;
        public ProjectileSO Projectile;
        public int ProjectileCount;
        public float AngleCoverage;
        public AnimationCurve SpeedMultiplierCurve;
        public float GetSpeed(float progress)
        {
            return SpeedMultiplierCurve.Evaluate(progress);
        }
        public float AngleIncrement => AngleCoverage / (ProjectileCount - 1);
    }
}
