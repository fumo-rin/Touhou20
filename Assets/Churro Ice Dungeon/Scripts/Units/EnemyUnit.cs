using Bremsengine;
using Core.Extensions;
using System.Collections;
using System.Diagnostics.Contracts;
using UnityEngine;

namespace ChurroIceDungeon
{
    #region Attack
    public partial class EnemyUnit
    {
        [SerializeField] float maxAttackDistance = 8f;
        public bool CanSeeAttackTarget => canSeeTarget && scanTarget != null;
        public void SetAttackDistance(float distance)
        {
            maxAttackDistance = distance;
        }
        public AttackHandler attackHandler;
        public void TryAttack(Vector2 worldPosition)
        {
            if (attackHandler.TryAttack(worldPosition))
            {
                SetStallTime(attackHandler.settings.StallDuration);
                attackHandler.settings.TriggerAttackTime();
                StallEndTime = attackHandler.settings.NewStallTime;
            }
        }
        private void AttackLoop()
        {
            if (CanSeeAttackTarget && scanTarget != null && scanTarget.IsAlive() && targetDistance <= maxAttackDistance)
            {
                TryAttack(scanTarget.AimTarget);
            }
        }
    }
    #endregion
    #region Strafe
    public partial class EnemyUnit
    {
        public void SetStrafeProfile(StrafeProfile strafe)
        {
            strafeProfile = strafe;
        }
        [SerializeField] StrafeProfile strafeProfile;
        float targetDistance;
        float strafeFlipTime;
        bool flipStrafe;
        bool TryGetStrafeVector(out Vector2 strafe)
        {
            strafe = Vector2.zero;
            if (strafeProfile == null)
            {
                return false;
            }
            if (ShouldStrafe)
            {
                pather.ClearPath();
                DungeonUnit strafeTarget = scanTarget;
                if (strafeProfile.TrySolveDistance(targetDistance, out StrafeProfile.Entry entry))
                {
                    if (Time.time >= strafeFlipTime)
                    {
                        flipStrafe = !flipStrafe;
                        strafeFlipTime = Time.time + entry.strafeFlipTimeRange.RandomBetweenXY();
                    }
                    strafe = scanTarget.CurrentPosition - CurrentPosition;
                    strafe = strafe.Rotate2D(flipStrafe ? -entry.strafeAngle : entry.strafeAngle);

                }
            }
            strafe = strafe.normalized;
            return strafe != Vector2.zero;
        }
    }
    #endregion
    #region Damageable
    public partial class EnemyUnit
    {
        [SerializeField] DestructionItem OptionalDestructionItem;
        public override bool IsAlive()
        {
            return gameObject.activeInHierarchy && Health > 0f;
        }
        protected override void OnHurt(float damage, Vector2 damagePosition)
        {
            if (!IsAlive())
            {
                return;
            }
            WakaScoring.SpawnPickup(CurrentPosition + Random.insideUnitCircle);
            Damageable.CurrentHealth -= damage;
            if (Damageable.CurrentHealth <= 0f)
            {
                if (OptionalDestructionItem != null && !OptionalDestructionItem.isDestroyed)
                {
                    OptionalDestructionItem.DestroyItem();
                }
                gameObject.SetActive(false);
            }
        }
    }
    #endregion
    #region Motor
    public partial class EnemyUnit
    {
        public DungeonMotor OverrideMotor;
        public override DungeonMotor CollapseMotor()
        {
            if (OverrideMotor != null)
            {
                return OverrideMotor;
            }
            return base.CollapseMotor();
        }
    }
    #endregion
    #region Stage Path
    public partial class EnemyUnit
    {
        StagePath activePath;
        public bool HasPath => activePath.IsPathValid;
        public bool TryRunPath()
        {
            if (!HasPath)
            {
                return false;
            }
            activePath.DrawPath();
            return activePath.PerformPath(this);
        }
        public void SetStagePath(StagePath path) => activePath = path;
    }
    #endregion
    public partial class EnemyUnit : DungeonUnit
    {
        [SerializeField] float Power = 0f;
        public override float DamageScale(float damage)
        {
            return ((1f + Power / 100f).Max(1f)) * damage;
        }
        DungeonUnit pathingTarget;
        DungeonUnit KnownTarget;
        [SerializeField] bool isBoss;
        float loseTargetTime;
        [SerializeField] float targetMemoryTime = 3f;
        [SerializeField] LayerMask targetBlockingLayer;
        [SerializeField] float targetVisionRange = 5f;
        Vector2[] addedScanPoints = new Vector2[5]
        {
            new(-0.25f,-0.25f),
            new(-0.25f,0.25f),
            new(0.25f,0.25f),
            new(0.25f,-0.25f),
            new(0f,0f)
        };
        RaycastHit2D targettingHit;

        private bool FindTarget(Vector2 scanPosition, out DungeonUnit output)
        {
            output = null;
            foreach (Vector2 point in addedScanPoints)
            {
                targettingHit = Physics2D.Raycast(CurrentPosition + point, scanPosition - (CurrentPosition + point), targetVisionRange, targetBlockingLayer);
                if (targettingHit.transform != null && targettingHit.transform.GetComponent<DungeonUnit>() is DungeonUnit validUnit and not null)
                {
                    output = validUnit;
                    return true;
                }
            }
            return false;
        }
        public void Alert(DungeonUnit aggressor)
        {
            SetKnownTarget(aggressor);
            if (pathingTarget != null)
            {
                return;
            }
            pathingTarget = aggressor;
            loseTargetTime = targetMemoryTime + Time.time;
            if (pather.HasPath)
            {
                pather.ClearPath();
            }
        }
        protected override void WhenAwake()
        {
            FactionInterface.SetFaction(BremseFaction.Enemy);
        }

        protected override void WhenDestroy()
        {
            TickManager.MainTickLightweight -= Tick;
        }
        public void SetKnownTarget(DungeonUnit target)
        {
            KnownTarget = target;
        }
        public bool HasTarget => KnownTarget != null;
        public void ForgetTarget()
        {
            KnownTarget = null;
        }
        [SerializeField] float initialStallTime = 1f;
        [SerializeField] AudioClipWrapper spawnSound;
        protected override void WhenStart()
        {
            SetStallTime(initialStallTime);
            spawnSound.Play(CurrentPosition);
            TickManager.MainTickLightweight += Tick;
            StartPatrol(Origin, 5f);
            if (ChurroManager.HardMode && attackHandler != null)
            {
                attackHandler.settings.SetStallDuration(attackHandler.settings.StallDuration * 0.66f);
                attackHandler.settings.SetSwingDuration(attackHandler.settings.SwingDuration * 0.66f);
            }
            if (isBoss)
            {
                Bossbar.BindBar(this);
            }
        }
        bool canSeeTarget = false;
        DungeonUnit scanTarget = null;
        public bool ShouldStrafe => canSeeTarget && scanTarget != null && scanTarget.IsAlive();
        float StallEndTime;
        public bool IsStalled => Time.time < StallEndTime;
        public void SetStallTime(float relativeDelay)
        {
            StallEndTime = Time.time + relativeDelay;
        }
        private void Tick()
        {
            if (IsStalled)
            {
                RB.VelocityTowards(Vector2.zero, 5f);
                return;
            }
            AttackLoop();

            if (gameObject.activeInHierarchy)
            {
                if (KnownTarget != null)
                {
                    canSeeTarget = FindTarget(KnownTarget.CurrentPosition, out scanTarget);
                }
                if (canSeeTarget)
                {
                    loseTargetTime = Time.time + targetMemoryTime;
                    if (pathingTarget == null && KnownTarget != null)
                    {
                        pathingTarget = scanTarget;
                    }
                    targetDistance = scanTarget.CurrentPosition.DistanceTo(CurrentPosition);
                    return;
                }

                if (pathingTarget != null && !pather.isAwaitingPath)
                {
                    SetDestination(pathingTarget.CurrentPosition);
                }
                else if (pathingTarget != null && Time.time > loseTargetTime)
                {
                    pathingTarget = null;
                    pather.ClearPath();
                    SetDestination(Origin);
                    QueuePatrol();
                    if (!CheckNavmeshPosition(Origin))
                    {
                        transform.position = Origin;
                    }
                }
            }
        }
        public void QueuePatrol()
        {
            pather.BindReachPathEndAction(QueuePatrolCoroutine);
        }
        private void QueuePatrolCoroutine()
        {
            StartPatrol(Origin, 5f);
        }
        private void StartPatrol(Vector2 target, float patrolRange)
        {
            Vector2 RandomPatrol()
            {
                return target + Random.insideUnitCircle * patrolRange;
            }
            IEnumerator CO_Patrol(float delay)
            {
                yield return new WaitForSeconds(delay);
                if (pathingTarget == null)
                {
                    bool hitNavmesh = false;
                    Vector2 patrol = Origin;
                    for (int i = 0; i < 10; i++)
                    {
                        patrol = RandomPatrol();
                        if (base.NavmeshContains(patrol))
                        {
                            hitNavmesh = true;
                        }
                        if (hitNavmesh)
                            break;
                        else
                        {
                            patrol = Origin;
                        }
                    }
                    pather.StartPathing(patrol);
                    QueuePatrol();
                }
            }
            StartCoroutine(CO_Patrol(1.5f.Spread(30f)));
        }
        private void Update()
        {
            DungeonMotor.MotorOutput result = new();
            if (IsStalled)
            {
                MoveMotor(Vector2.zero, out result);
                return;
            }
            if (TryRunPath())
            {
                return;
            }
            bool strafeSuccess = TryGetStrafeVector(out Vector2 strafe);
            if (strafeSuccess)
            {
                strafe = pather.rvo.SolveRVO(strafe, CollapseMotor() is DungeonMotor m and not null ? m.MaxSpeed : 1f);
                MoveMotor(strafe, out result);
                Debug.DrawLine(CurrentPosition, CurrentPosition + strafe, ColorHelper.DeepGreen, 0.1f);
                return;
            }
            if (!strafeSuccess && pather.HasPath && pather.PerformPath(out Vector2 pathVector))
            {
                MoveMotor(pathVector, out result);
                return;
            }
            MoveMotor(Vector2.zero, out result);
        }
    }
}
