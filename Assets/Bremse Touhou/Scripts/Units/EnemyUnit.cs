using Bremsengine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BremseTouhou
{
    public class EnemyUnit : BaseUnit
    {
        [SerializeField] bool isBoss;
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
    }
}
