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
    public partial class BaseUnit : IProjectileHitListener
    {
        public IFaction FactionInterface => ((IFaction)this);
        BremseFaction IFaction.Faction { get; set; }
        protected abstract bool ProjectileHit(Projectile p);
        bool IProjectileHitListener.OnProjectileHit(Projectile p)
        {
            return ProjectileHit(p);
        }
    }
    #endregion
    #region Health Bar
    public partial class BaseUnit
    {
        public string UnitName => unitName;
        public string unitName = "Headhunter, Leather Belt";
        public string HealthText => $"{CurrentHealth.Ceil().Max(0f)}/{MaxHealth.Ceil()}";
        public float MaxHealth => 10000f;
        public float CurrentHealth => unitHealth;
        private float unitHealth;

        public void ChangeHealth(float delta)
        {
            unitHealth += delta;
            OnHealthChange?.Invoke(this);
        }
    }
    #endregion
    [SelectionBase]
    [RequireComponent(typeof(Rigidbody2D))]
    public abstract partial class BaseUnit : MonoBehaviour
    {
        public static BaseUnit Player;
        public bool Alive;
        public static BaseUnit GameTarget => Player;
        [SerializeField] protected Transform unitCenterAnchor;
        public Vector2 Center => unitCenterAnchor == null ? (Vector2)transform.position + new Vector2(0f, 0.5f) : unitCenterAnchor.position;
        public Vector2 Up => Center + Vector2.up.Shift(5f);
        protected Rigidbody2D rb;
        [SerializeField] BremseFaction UnitFaction;
        public delegate void HealthEvent(BaseUnit unit);
        public HealthEvent OnHealthChange;
        private void Awake()
        {
            if (rb == null)
            {
                rb = GetComponent<Rigidbody2D>();
            }
            FactionInterface.SetFaction(UnitFaction);
            unitHealth = MaxHealth;
            OnAwake();
        }
        protected abstract void OnAwake();
    }
}
