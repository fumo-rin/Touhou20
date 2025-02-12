using Bremsengine;
using Core.Extensions;
using JetBrains.Annotations;
using NUnit.Framework.Internal.Commands;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
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
    #region Projectile Hit & Faction
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
        public void SetPhaseHandler(DeathPhase d)
        {
            phaseHandler = d;
        }
        DeathPhase phaseHandler;
        HashSet<AttackHandler> unitAttackHandlers = new();
        bool knownAliveState = true;
        private void ReFindAttackHandlers()
        {
            unitAttackHandlers.Clear();
            foreach (AttackHandler handler in transform.root.GetComponentsInChildren<AttackHandler>())
            {
                if (handler == null)
                    continue;
                unitAttackHandlers.Add(handler);
            }
            foreach (AttackHandler handler in transform.root.GetComponents<AttackHandler>())
            {
                if (handler == null) continue;
                unitAttackHandlers.Add(handler);
            }
            RecalculateAttackHandlerState();
        }
        private void RecalculateAttackHandlerState()
        {
            if (unitHealth <= 0f)
            {
                knownAliveState = false;
            }
            else
            {
                knownAliveState = true;
            }
            foreach (AttackHandler handler in unitAttackHandlers)
            {
                handler.SetAlive(knownAliveState);
            }
        }
        public string UnitName => unitName;
        public string unitName = "Headhunter, Leather Belt";
        public string HealthText => $"{CurrentHealth.Ceil().Max(0f)}/{MaxHealth.Ceil()}";
        public float MaxHealth = 1000f;
        public float CurrentHealth => unitHealth;
        private float unitHealth;
        public void Hurt(float damage, Vector2 damagePosition)
        {
            ChangeHealth(-(damage.Absolute()));
        }
        public void ChangeHealth(float delta)
        {
            if (unitHealth <= 0f)
                return;

            float value = (unitHealth + delta).Max(0f);
            unitHealth = value;
            RecalculateAttackHandlerState();
            if (unitHealth <= 0f)
            {
                Kill(killSettings);
            }
            OnHealthChange?.Invoke(this);
        }
        public void SetNewHealth(float newHealth, float maxHealth, KillSettings? killOverride = null)
        {
            bool wasAlive = Alive;
            this.MaxHealth = maxHealth;
            this.unitHealth = newHealth;
            if (unitHealth <= 0f && wasAlive)
            {
                Kill(killOverride != null ? (KillSettings)killOverride : killSettings);
            }
            OnHealthChange?.Invoke(this);
            RecalculateAttackHandlerState();
        }
        public void ForceKill(KillSettings killOverride)
        {
            SetNewHealth(0, MaxHealth, killOverride);
        }
        [System.Serializable]
        public struct KillSettings
        {
            public bool DropLoot => Helper.SeededRandomInt256 < BulletCancelLootWeight;
            public KillSettings(int lootWeight)
            {
                this.BulletCancelLootWeight = lootWeight;
                this.bulletSweepTimeDuration = 0f;
                this.BypassPhase = false;
                this.ClearFactionProjectiles = false;
            }
            [Range(0f, 2f)]
            public float bulletSweepTimeDuration;
            [Range(0, 256)]
            [SerializeField] int BulletCancelLootWeight;
            public bool BypassPhase;
            public bool ClearFactionProjectiles;
        }
        private void Kill(KillSettings settings)
        {
            if (phaseHandler != null && !settings.BypassPhase)
            {
                SpellCardUI.CompleteSpell();
                phaseHandler.SetNextPhase();
                if (phaseHandler.ShouldDie())
                {
                    if (gameObject != null)
                    {
                        gameObject.SetActive(false);
                        BossManager.Release(this);
                        if (this is EnemyUnit enemy)
                        {
                            if (enemy.isBoss)
                            {
                                TouhouManager.PlayBossKillSound(enemy.Center);
                            }
                        }
                    }
                }
                else
                {
                    PhaseEntry entry = phaseHandler.GetPhase();
                    float newHealth = entry.MaxHealth;
                    SetNewHealth(newHealth, newHealth);
                }
            }
            else
            {
                gameObject.SetActive(false);
                BossManager.Release(this);
            }
            if (transform != null)
                Projectile.ClearProjectileTimelineFor(transform);

            if (settings.ClearFactionProjectiles)
            {
                Projectile.SetSpawnSweepTime(settings.bulletSweepTimeDuration);
                Projectile.ClearProjectilesOfFaction(BremseFaction.Enemy, settings.DropLoot ? PlayerScoring.SpawnPickup : null);
            }
        }
    }
    #endregion
    #region Target
    public partial class BaseUnit
    {
        BaseUnit target;
        public BaseUnit Target => target;
        public Transform TargetTransform => Target == null ? null : Target.transform;
        public bool HasTarget => Target != null;
        [SerializeField] KillSettings killSettings;
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
    #region Triggerboxes
    public abstract partial class BaseUnit
    {
        [SerializeField] List<TargetBox> targetBoxes = new();
        private void BindTargetBoxes()
        {
            foreach (var item in targetBoxes)
            {
                if (item == null)
                    continue;
                item.OnTakeDamage += Hurt;
            }
        }
        private void ReleaseTargetBoxes()
        {
            foreach (var item in targetBoxes)
            {
                if (item == null)
                    continue;
                item.OnTakeDamage -= Hurt;
            }
        }
    }
    #endregion
    #region Stage Progress
    public abstract partial class BaseUnit
    {
        int deleteOnProgress = -1;
        public BaseUnit SetProgressDeletion(int progress)
        {
            deleteOnProgress = progress;
            return this;
        }
        public bool ShouldProgressDelete(int progress)
        {
            return deleteOnProgress >= 0 && progress >= deleteOnProgress;
        }
        private void TryProgressDelete(int stageProgress)
        {
            if (Alive && ShouldProgressDelete(stageProgress))
            {
                Debug.Log("Progress Delete : " + ShouldProgressDelete(stageProgress));
                KillSettings k = new(0);
                k.BypassPhase = true;
                ForceKill(k);
            }
        }
        private void BindProgressDeleteEvent()
        {
            TouhouManager.OnSetProgress += TryProgressDelete;
        }
        private void ReleaseProgressDeleteEvent()
        {
            TouhouManager.OnSetProgress -= TryProgressDelete;
        }
    }
    #endregion
    [SelectionBase]
    [RequireComponent(typeof(Rigidbody2D))]
    [DefaultExecutionOrder(100)]
    public abstract partial class BaseUnit : MonoBehaviour
    {
        public static BaseUnit Player;
        public bool Alive => CurrentHealth > 0f && gameObject.activeInHierarchy;
        public static BaseUnit GameTarget => Player;
        [SerializeField] protected Transform unitCenterAnchor;
        public Vector2 Center => unitCenterAnchor == null ? (Vector2)transform.position + new Vector2(0f, 0.5f) : unitCenterAnchor.position;
        public Vector2 Up => Center + Vector2.up.ScaleToMagnitude(5f);
        protected Rigidbody2D rb;
        [SerializeField] BremseFaction UnitFaction;
        public delegate void HealthEvent(BaseUnit unit);
        public HealthEvent OnHealthChange;
        public HealthEvent OnDeath;
        [SerializeField] Collider2D unitCollider;
        public Collider2D Collider => unitCollider;
        public Vector2 CurrentVelocity => rb.linearVelocity;
        public static LayerMask EnemyLayer { get; } = 1 << 6;
        public Vector2 Origin { get; private set; }
        private void Awake()
        {
            Origin = Center;
            if (rb == null)
            {
                rb = GetComponent<Rigidbody2D>();
            }
            FactionInterface.SetFaction(UnitFaction);
            SetNewHealth(MaxHealth, MaxHealth);
            ReFindAttackHandlers();
            WhenAwake();
        }
        private void Start()
        {
            BindTargetBoxes();
            WhenStart();
            BindProgressDeleteEvent();
        }
        private void OnDestroy()
        {
            ReleaseTargetBoxes();
            WhenDestroy();
            ReleaseProgressDeleteEvent();
        }
        protected abstract void WhenDestroy();
        protected abstract void WhenStart();
        protected abstract void WhenAwake();
    }
}
