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
            float small = 360f / (Hardmode ? 15f : 7f);
            float big = 360f / (Hardmode ? 24f : 8f);
            ChurroProjectile.ArcSettings bigArc = new(0f, 360f, big, Hardmode ? 3f : 4.5f);
            ChurroProjectile.ArcSettings smallArc = new(0f, 360f, small, Hardmode ? 3f : 6f);

            ChurroProjectile.SpawnArc(bigPrefab, input, bigArc, out iterationList);
            foreach (var item in iterationList)
            {
                item.Action_AddPosition(item.CurrentVelocity.ScaleToMagnitude(1.5f));
                if (ignoreProjectileCollider)
                {
                    Physics2D.IgnoreCollision(ignoreProjectileCollider, item.ProjectileCollider);
                }
            }
            ChurroProjectile.SpawnArc(prefab, input, smallArc, out _);
            StartCoroutine(CO_ExtraSmallRings(4, small / 4f));
            IEnumerator CO_ExtraSmallRings(int count, float rotationStep)
            {
                ChurroProjectile.InputSettings reverse = new(input.Origin, input.Direction)
                {
                    OnSpawn = input.OnSpawn,
                };
                for (int i = 0; i < count; i++)
                {
                    yield return new WaitForSeconds(Hardmode ? 0.2f : 0.065f);
                    input.SetDirection(input.Direction.Rotate2D(rotationStep));
                    ChurroProjectile.SpawnArc(prefab, input, smallArc, out iterationList);
                    foreach (var item in iterationList)
                    {
                        attackSound.Play(transform.position);
                        item.Action_AddPosition(item.CurrentVelocity.ScaleToMagnitude(-2.5f));
                        if (ignoreProjectileCollider)
                        {
                            Physics2D.IgnoreCollision(ignoreProjectileCollider, item.ProjectileCollider);
                        }
                    }
                    if (!Hardmode)
                    {
                        continue;
                    }
                    reverse.SetDirection(reverse.Direction.Rotate2D(-rotationStep)); 
                    ChurroProjectile.SpawnArc(prefab, reverse, smallArc, out iterationList);
                    foreach (var item in iterationList)
                    {
                        attackSound.Play(transform.position);
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
