using Bremsengine;
using Core.Extensions;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace BremseTouhou
{
    #region Motor Movement
    public abstract partial class BaseUnit
    {
        [SerializeField] protected UnitMotor standardMotor;
        protected float nextMoveTime;
        List<Vector2> currentPath;
        public virtual UnitMotor ActiveMotor => standardMotor;
        protected bool HasPath => currentPath != null && currentPath.Count > 0;
        public void Move(UnitMotor motor, Vector2 input)
        {
            Vector2 direction = input.normalized;
            motor.Move(rb, direction, ref nextMoveTime);
        }
        protected void ProcessCurrentPath()
        {
            if (currentPath == null || currentPath.Count <= 0)
            {
                return;
            }
            while (currentPath.Count > 0 && currentPath[0].SquareDistanceToLessThan(UnitCenter, 1f))
            {
                currentPath.RemoveAt(0);
            }
        }
        protected void SetPath(List<Vector2> path)
        {
            currentPath = path.ToList();
        }
    }
    #endregion
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
            Debug.Log("Apply Projectile");
            Debug.Log(FactionInterface.Faction.ToString());
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
        bool IProjectileHitListener.OnProjectileHit(Projectile p)
        {
            return ProjectileHit(p);
        }
    }
    #endregion
    [SelectionBase]
    [RequireComponent(typeof(Rigidbody2D))]
    public abstract partial class BaseUnit : MonoBehaviour
    {
        Rigidbody2D rb;
        [SerializeField] BremseFaction UnitFaction;
        private void OnValidate()
        {
            if (rb == null)
            {
                rb = GetComponent<Rigidbody2D>();
            }
            FactionInterface.SetFaction(UnitFaction);
        }
    }
}
