using Bremsengine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BremseTouhou
{
    [CreateAssetMenu(menuName = "Bremse Touhou/Attack/Projectile")]
    public class ProjectileAttack : UnitAttack
    {
        [SerializeField] ProjectileSO projectile;
        [SerializeField] List<ProjectileEvent> projectileEvents = new();
        public override void AttackTarget(BaseUnit owner, Vector2 origin, Vector2 target, float addedAngle = 0f)
        {
            PlaySound(owner);
            ProjectileDirection d = new(projectile, target - origin);
            d.AddAngle(addedAngle);
            Projectile p = Projectile.SpawnProjectile(projectile, origin, d, OnProjectileSpawn);
            p.SetFaction(owner == null ? BremseFaction.None : owner.FactionInterface.Faction);
            if (owner != BaseUnit.GameTarget)
            {
                foreach (ProjectileEvent e in projectileEvents)
                {
                    e.QueueEvent(p, BaseUnit.GameTarget);
                }
            }
        }
    }
}
