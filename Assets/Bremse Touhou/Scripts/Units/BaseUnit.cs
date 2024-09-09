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
        public void Move(UnitMotor motor, Vector2 input, float scaler = 1f)
        {
            Debug.Log($"motor : {motor.ToString()} input : {input.ToString("F1")}");
            Vector2 direction = input.normalized;
                //* (input.magnitude * scaler).Min(1f);
            motor.Move(rb, direction, ref nextMoveTime);
        }
        protected void ProcessCurrentPath()
        {
            if (currentPath == null || currentPath.Count <= 0)
            {
                return;
            }
            while (currentPath.Count > 0 && currentPath[0].SquareDistanceToLessThan(Center, 1f))
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
            ProjectileDirection direction = new(p,target - Center);
            Projectile.SpawnProjectile(p, Center, direction, ApplyProjectile);
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
        public Vector2 Center => unitCenterAnchor == null ? (Vector2)transform.position + new Vector2(0f,0.5f) : unitCenterAnchor.position;
        public Vector2 Up => Center + Vector2.up.Shift(5f);
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
        protected Rigidbody2D rb;
        [SerializeField] BremseFaction UnitFaction;
        private void Awake()
        {
            if (rb == null)
            {
                rb = GetComponent<Rigidbody2D>();
            }
            FactionInterface.SetFaction(UnitFaction);
            OnAwake();
        }
        protected abstract void OnAwake();
    }
}
