using Bremsengine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BremseTouhou
{
    public abstract class UnitAttack : ScriptableObject
    {
        [SerializeField] float damage = 10f;
        public abstract void AttackTarget(BaseUnit owner, Vector2 origin,Vector2 target);
        protected void OnProjectileSpawn(Projectile p)
        {
            p.SetDamage(damage);
        }
    }
}