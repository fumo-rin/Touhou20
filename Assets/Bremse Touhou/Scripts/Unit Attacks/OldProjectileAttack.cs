using Bremsengine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BremseTouhou
{
    [CreateAssetMenu(menuName = "Bremse Touhou/Attack/Projectile")]
    public class OldProjectileAttack : UnitAttack
    {
        /// <summary>
        /// THIS SCRIPT IS OLD SEE BREMSENGINE/ProjectileAttack.cs for the Graph Version
        /// </summary>
        [SerializeField] ProjectileSO projectile;
        [SerializeField] List<ProjectileEvent> projectileEvents = new();
        public override void AttackTarget(BaseUnit owner, Vector2 origin, Vector2 target, float addedAngle = 0f)
        {
            PlaySound(owner);
            ProjectileDirection d = new(projectile, target - origin);
            d.AddAngle(addedAngle);
            Projectile p = Projectile.SpawnProjectile(projectile, owner.transform, origin, d, OnProjectileSpawn, owner.TargetTransform);
            p.SetFaction(owner == null ? BremseFaction.Enemy : owner.FactionInterface.Faction);

            foreach (ProjectileEvent e in projectileEvents)
            {
                e.QueueEvent(p, owner, owner.Target);
            }
        }
    }
}
