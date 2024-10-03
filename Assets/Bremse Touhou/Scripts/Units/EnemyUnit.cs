using Bremsengine;
using Core.Extensions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace BremseTouhou
{
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
        [SerializeField] EnemyAttackHandler targetHandler;
        [field: SerializeField] public bool isBoss { get; private set; }
        protected override void OnAwake()
        {
            if (isBoss)
            {
                BossManager.Bind(this);
            }
            if (SetTarget(StageTarget.TargetUnit) is BaseUnit u and not null)
            {
                if (targetHandler != null)
                {
                    targetHandler.AssignTarget(u.transform);
                }
            }
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
