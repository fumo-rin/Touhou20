using Bremsengine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BremseTouhou
{
    #region Shoot Projectile
    public abstract partial class BaseUnit
    {
        public void ShootProjectile(ProjectileSO p, Vector2 target)
        {
            ProjectileDirection direction = new(p,target - UnitCenter);
            Projectile.SpawnProjectile(p, UnitCenter, direction, ApplyProjectile);
        }
        public void ApplyProjectile(Projectile p)
        {
            p.SetFaction(FactionInterface.Faction);
            p.SetDamage(10);
        }
    }
    #endregion
    #region Projectile Hit
    public abstract partial class BaseUnit : IProjectileHitListener
    {
        [SerializeField] protected Transform unitCenterAnchor;
        public Vector2 UnitCenter => unitCenterAnchor == null ? (Vector2)transform.position + new Vector2(0f,0.5f) : unitCenterAnchor.position;
        public IFaction FactionInterface => ((IFaction)this);
        BremseFaction IFaction.Faction { get; set; }
        protected abstract bool ProjectileHit(Projectile p);
        public bool OnProjectileHit(Projectile p)
        {
            return ProjectileHit(p);
        }
        bool IProjectileHitListener.OnProjectileHit(Projectile p)
        {
            throw new System.NotImplementedException();
        }
    }
    #endregion
    public abstract partial class BaseUnit : MonoBehaviour
    {

    }
}
