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
        public void StopMovement()
        {
            rb.linearVelocity = Vector2.zero;
        }
        public void Friction(float friction)
        {
            rb.VelocityTowards(Vector2.zero, friction);
        }
    }
    #endregion
    #region Unit Path
    public partial class BaseUnit
    {
        Coroutine currentUnitPath;
        public void StartUnitPathCoroutine(Coroutine c)
        {
            EndUnitPathCoroutine();
            currentUnitPath = c;
        }
        private void EndUnitPathCoroutine()
        {
            if (currentUnitPath == null)
                return;
            StopCoroutine(currentUnitPath);
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
    #region Health & Activity
    public partial class BaseUnit
    {
        public string UnitName => unitName;
        public string unitName = "Headhunter, Leather Belt";
        public string HealthText => $"{CurrentHealth.Ceil().Max(0f)}/{MaxHealth.Ceil()}";
        public float MaxHealth => 1000f;
        public float CurrentHealth => unitHealth;
        private float unitHealth;
        public void ChangeHealth(float delta)
        {
            if (unitHealth <= 0f)
                return;

            unitHealth = (unitHealth + delta).Max(0f);
            if (unitHealth <= 0f)
            {
                Kill();
            }
            OnHealthChange?.Invoke(this);
        }
        public void SetActive(bool state)
        {
            Active = state;
        }
        private void Kill()
        {
            Debug.Log("Death");
            gameObject.SetActive(false);
            if (this is EnemyUnit enemy)
            {
                if (enemy.isBoss)
                {
                    TouhouManager.PlayBossKillSound();
                    Projectile.ClearProjectilesOfFaction(BremseFaction.Enemy);
                }
            }
            Projectile.ClearProjectileTimelineFor(transform);
        }
    }
    #endregion
    #region Target
    public partial class BaseUnit
    {
        BaseUnit target;
        public BaseUnit Target =>  target;
        public Transform TargetTransform => Target == null ? null : Target.transform;
        public bool HasTarget => Target != null;
        public BaseUnit SetTarget(BaseUnit t)
        {
            CancelLoseTarget();
            target = t;
            OnTargetUpdate?.Invoke(t);
            return t;
        }
        public bool IsTarget(BaseUnit t) => target == t;
        Coroutine loseTargetCoroutine;
        public void CancelLoseTarget()
        {
            if (loseTargetCoroutine != null)
            {
                StopCoroutine(loseTargetCoroutine);
            }
            loseTargetCoroutine = null;
        }
        public void LoseTarget(float delayInSeconds)
        {
            if (gameObject.activeInHierarchy)
            {
                loseTargetCoroutine = StartCoroutine(CO_LoseTarget(delayInSeconds));
            }
        }
        public delegate void TargetRefresh(BaseUnit newTarget);
        protected TargetRefresh OnTargetUpdate;
        private IEnumerator CO_LoseTarget(float delayInSeconds)
        {
            yield return new WaitForSeconds(delayInSeconds);
            loseTargetCoroutine = null;
            SetTarget(null);
        }
    }
    #endregion
    [SelectionBase]
    [RequireComponent(typeof(Rigidbody2D))]
    [DefaultExecutionOrder(100)]
    public abstract partial class BaseUnit : MonoBehaviour
    {
        public static BaseUnit Player;
        public bool Alive => CurrentHealth > 0f;
        public bool Active;
        public static BaseUnit GameTarget => Player;
        [SerializeField] protected Transform unitCenterAnchor;
        public Vector2 Center => unitCenterAnchor == null ? (Vector2)transform.position + new Vector2(0f, 0.5f) : unitCenterAnchor.position;
        public Vector2 Up => Center + Vector2.up.ScaleToMagnitude(5f);
        protected Rigidbody2D rb;
        [SerializeField] BremseFaction UnitFaction;
        public delegate void HealthEvent(BaseUnit unit);
        public HealthEvent OnHealthChange;
        [SerializeField] Collider2D unitCollider;
        public Collider2D Collider => unitCollider;
        public Vector2 CurrentVelocity => rb.linearVelocity;
        public static LayerMask EnemyLayer { get; } = 1 << 6;
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
