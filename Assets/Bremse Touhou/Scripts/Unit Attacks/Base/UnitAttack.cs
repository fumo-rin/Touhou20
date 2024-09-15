using Bremsengine;
using Core.Extensions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BremseTouhou
{
    public abstract class UnitAttack : ScriptableObject
    {
        public abstract void AttackTarget(BaseUnit owner, Vector2 origin, Vector2 target, float addedAngle = 0f);
        protected void OnProjectileSpawn(Projectile p, Transform owner, Transform target)
        {
            p.SetDamage(damage);
            if (target != null)
            {
                p.SetTarget(target);
            }
            if (owner.transform.GetComponent<BaseUnit>() is BaseUnit unit)
            {
                p.SetIgnoreCollision(unit.Collider);
            }
        }
        [SerializeField] float damage = 10f;
        [SerializeField] AudioClipWrapper AttackSound;
        public void PlaySound(BaseUnit owner)
        {
            AttackSound.Play(owner == null ? Vector2.zero : owner.Center);
        }
    }
}