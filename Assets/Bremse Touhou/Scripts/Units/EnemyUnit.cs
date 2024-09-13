using Bremsengine;
using Core.Extensions;
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
        private Vector2 Funni()
        {
            Vector2 funni = ((Vector2)(new Vector3().X(10f).Y(10f)))
                .Rotate2D(10f)
                .Quantize(2f)
                .ScaleToMagnitude(5f)
                .Bounce(Vector2.up, 1f)
                .Rotate2D(360f);
            return funni;
        }
    }
}
