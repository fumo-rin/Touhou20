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
        public override void AttackTarget(BaseUnit owner, Vector2 origin, Vector2 target)
        {
            ProjectileDirection d = new(projectile, target - origin);
            Projectile p = Projectile.SpawnProjectile(projectile, origin, d, OnProjectileSpawn);
            p.SetFaction(owner == null ? BremseFaction.None : owner.FactionInterface.Faction);
        }
    }
}
