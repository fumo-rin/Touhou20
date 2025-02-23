using Core.Extensions;
using System.Collections;
using UnityEngine;

namespace ChurroIceDungeon
{
    public class MakaiAttack : ChurroBaseAttack
    {
        [SerializeField] ChurroProjectile prefab;
        [SerializeField] ChurroProjectile bigPrefab;
        [SerializeField] Collider2D ignoreProjectileCollider;
        protected override void AttackPayload(ChurroProjectile.InputSettings input)
        {
            float small = 360f / (Hardmode ? 30f : 12f);
            float big = 360f / (Hardmode ? 24f : 8f);
            ChurroProjectile.ArcSettings bigArc = new(0f, 360f, big, Hardmode ? 4.5f : 3f);
            ChurroProjectile.ArcSettings smallArc = new(0f, 360f, small, Hardmode ? 6 : 4f);

            foreach (var item in ChurroProjectile.SpawnArc(bigPrefab, input, bigArc))
            {
                item.Action_AddPosition(item.CurrentVelocity.ScaleToMagnitude(1.5f));
                if (ignoreProjectileCollider)
                {
                    Physics2D.IgnoreCollision(ignoreProjectileCollider, item.ProjectileCollider);
                }
            }
            ChurroProjectile.SpawnArc(prefab, input, smallArc);
            StartCoroutine(CO_ExtraSmallRings(4, small / 4f));
            IEnumerator CO_ExtraSmallRings(int count, float rotationStep)
            {
                for (int i = 0; i < count * (Hardmode ? 3f :1f); i++)
                {
                    yield return new WaitForSeconds(0.2f * (Hardmode ? 0.33f : 1f));
                    input.SetDirection(input.Direction.Rotate2D(rotationStep * (Hardmode ? 3f: 1f)));
                    foreach (var item in ChurroProjectile.SpawnArc(prefab, input, smallArc))
                    {
                        item.Action_AddPosition(item.CurrentVelocity.ScaleToMagnitude(-2.5f));
                        if (ignoreProjectileCollider)
                        {
                            Physics2D.IgnoreCollision(ignoreProjectileCollider, item.ProjectileCollider);
                        }
                    }
                }
            }
        }
    }
}
