using Core.Extensions;
using System.Collections.Generic;
using UnityEngine;

namespace ChurroIceDungeon
{
    public class BarrelAttack : ChurroBaseAttack
    {
        [SerializeField] ChurroProjectile prefab;
        [SerializeField] ChurroProjectile bigPrefab;
        [SerializeField] Collider2D ignoreProjectileCollider;
        protected override void AttackPayload(ChurroProjectile.InputSettings input)
        {
            ChurroProjectile.ArcSettings bigArc = new(0f, 360f, 72f, 2.5f);
            ChurroProjectile.ArcSettings smallArc = new(0f, 360f, 30f, 2.5f);
            if (!Hardmode)
            {
                smallArc = smallArc * 0.6f;
                bigArc = bigArc * 0.75f;
            }

            foreach (var item in ChurroProjectile.SpawnArc(bigPrefab, input, bigArc))
            {
                item.Action_AddPosition(item.CurrentVelocity.ScaleToMagnitude(1.25f));
                if (ignoreProjectileCollider)
                {
                    Physics2D.IgnoreCollision(ignoreProjectileCollider, item.ProjectileCollider);
                }
            }
            ChurroProjectile.SpawnArc(prefab, input, smallArc);
        }
    }
}
