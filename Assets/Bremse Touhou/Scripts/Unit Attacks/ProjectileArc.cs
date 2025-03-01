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
        [SerializeField] bool mirrored;
        [SerializeField] List<ProjectileEvent> projectileEvents = new();
        public override void AttackTarget(BaseUnit owner, Vector2 origin, Vector2 target, float addedAngle)
        {
            PlaySound(owner);
            ProjectileDirection directionIteration;
            float progress;
            int iteration;
            float iterationAngle;
            foreach (ProjectileArcEntry entry in arc)
            {
                progress = 0f;
                iteration = 0;
                iterationAngle = (entry.startingAngle + (entry.ProjectileCount > 1 ? -entry.AngleCoverage.Multiply(0.5f) : 0f)) * mirrored.AsFloat(1f, -1f);
                for (int i = 0; i < entry.ProjectileCount; i++)
                {
                    directionIteration = new ProjectileDirection(entry.Projectile, target - origin)
                        .AddAngle(addedAngle);

                    directionIteration.AddAngle(entry.IsRandomAngle ? entry.RandomAngle : iterationAngle);

                    directionIteration.SetSpeedMod(entry.GetSpeed(progress));

                    SpawnProjectile(entry.Projectile, owner, origin, directionIteration);

                    iterationAngle += entry.AngleIncrement * mirrored.AsFloat(1f, -1f);
                    iteration++;
                    progress = iteration == 0 ? 0f : ((float)iteration / ((float)entry.ProjectileCount - 1)).Clamp(0f, 1f);
                }
            }
        }
        private Projectile SpawnProjectile(ProjectileSO projectile, BaseUnit owner, Vector2 origin, ProjectileDirection d)
        {
            Projectile p = Projectile.SpawnProjectile(projectile, !owner ? null : owner.transform, origin, d, OnProjectileSpawn, owner.TargetTransform);
            p.SetFaction(owner == null ? BremseFaction.Enemy : owner.FactionInterface.Faction);

            foreach (ProjectileEvent e in projectileEvents)
            {
                e.QueueEvent(p, owner, owner.Target);
            }
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
        [SerializeField] public bool IsRandomAngle;
        public float RandomAngle => startingAngle.AddRandomBetween(-AngleCoverage * 0.5f, AngleCoverage * 0.5f);
        public float GetSpeed(float progress)
        {
            return SpeedMultiplierCurve.Evaluate(progress);
        }
        public float AngleIncrement => AngleCoverage / (ProjectileCount - (AngleCoverage < 360 ? 1 : 0));
    }
}
