using Bremsengine;
using Core.Extensions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace BremseTouhou
{
    public class EnemyUnit : BaseUnit
    {
        [field: SerializeField] public bool isBoss { get; private set; }
        protected override void OnAwake()
        {
            if (isBoss)
            {
                BossManager.Bind(this);
            }
        }
        protected override bool ProjectileHit(Projectile p)
        {
            if (FactionInterface.IsFriendsWith(p.Faction))
            {
                return false;
            }
            ChangeHealth(-p.Damage);
            return true;
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
