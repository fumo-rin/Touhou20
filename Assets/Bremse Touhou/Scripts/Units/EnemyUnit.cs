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
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Reinitialize()
        {
            NextHitSoundTime = 0f;
        }
        static float NextHitSoundTime;
        static float MinimumHitSoundInterval = 0.025f;
        [SerializeField] AudioClipWrapper HitSound;
        [SerializeField] AudioClipWrapper LowHitSound;
        private void PlayHitSound(BaseUnit unit)
        {
            if (Time.time < NextHitSoundTime)
            {
                return;
            }
            NextHitSoundTime = Time.time + MinimumHitSoundInterval;
            if (unit.CurrentHealth < ((EnemyUnit)unit).lowHealth)
            {
                LowHitSound.Play(unit.Center);
                return;
            }
            HitSound.Play(unit.Center);
        }
    }
    #endregion
    public partial class EnemyUnit : BaseUnit
    {
        [SerializeField] EnemyAttackHandler targetHandler;
        [field: SerializeField] public bool isBoss { get; private set; }
        protected override void WhenAwake()
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
        public float lowHealth => MaxHealth * 0.15f;
        protected override void WhenStart()
        {
            OnHealthChange += PlayHitSound;
        }
        protected override void WhenDestroy()
        {
            OnHealthChange -= PlayHitSound;
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
