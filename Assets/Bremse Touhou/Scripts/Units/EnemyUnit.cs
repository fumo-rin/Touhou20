using Bremsengine;
using Core.Extensions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace BremseTouhou
{
    #region Scan Target
    public partial class EnemyUnit
    {
        [SerializeField] TargetScanner scanner;
        private void ScanTick()
        {
            if (scanner == null)
                return;

            if (Target == null && scanner.TryScan(out BaseUnit foundUnit))
            {
                SetTarget(foundUnit);
            }
        }
    }
    #endregion
    #region Projectile Hit
    public partial class EnemyUnit
    {
        protected override bool ProjectileHit(Projectile p)
        {
            if (FactionInterface.IsFriendsWith(p.Faction))
            {
                return false;
            }
            ChangeHealth(-p.Damage);
            return true;
        }
    }
    #endregion
    public partial class EnemyUnit : BaseUnit
    {
        [field: SerializeField] public bool isBoss { get; private set; }
        protected override void OnAwake()
        {
            if (isBoss)
            {
                BossManager.Bind(this);
            }
        }
        private void Start()
        {
            TickManager.AIThinkTick += ScanTick;
        }
        private void OnDestroy()
        {
            TickManager.AIThinkTick -= ScanTick;
        }
        float nextAttackTime;
        [SerializeField] float addedAttackDelay = 0.4f;
        [SerializeField] UnityEvent testAttackEvent;
        private void Update()
        {
            if (nextAttackTime <= Time.time)
            {
                nextAttackTime = Time.time + addedAttackDelay;
                testAttackEvent?.Invoke();
            }
        }
    }
}
