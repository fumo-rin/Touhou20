using Core.Extensions;
using System.Collections.Generic;
using System.Collections;
using UnityEngine;
namespace ChurroIceDungeon
{
    public class JimboAttack : ChurroBaseAttack
    {
        [SerializeField] List<Transform> attackOrigins = new();
        [SerializeField] float phase3Health;
        int iteration;
        [SerializeField] ChurroProjectile prefabP1;
        [SerializeField] ChurroProjectile prefabP3;
        [SerializeField] int repeats = 2;
        protected override void WhenStart()
        {
            base.WhenStart();
        }
        private Transform GetAttackTransform => attackOrigins[iteration % attackOrigins.Count];
        protected override void AttackPayload(ChurroProjectile.InputSettings input)
        {
            void Attack1(ChurroProjectile.InputSettings input)
            {
                for (int i = 0; i < repeats; i++)
                {
                    float degrees = Hardmode ? 45f : 60f;
                    float offset = Random.Range(0f, degrees);
                    input.SetOrigin(GetAttackTransform.position);
                    ChurroProjectile.ArcSettings ring = new((0f + offset).Clamp(0f, 360f), 360f + offset, degrees, Hardmode ? 4f : 2f);
                    ChurroProjectile.SpawnArc(prefabP1, input, ring);
                    iteration = iteration + 1;
                }
            }
            IEnumerator Attack2(ChurroProjectile.InputSettings input)
            {
                yield return new WaitForSeconds(0.15f);
                if (owner == null || !owner.IsAlive())
                    yield break;
                attackSound.Play(GetAttackTransform.position);
                for (int i = 0; i < repeats ; i++)
                {
                    float degrees = Hardmode ? 360f / 16f : 360f / 24f;
                    float offset = Random.Range(0f, degrees);
                    input.SetOrigin(GetAttackTransform.position);
                    ChurroProjectile.ArcSettings ring = new((0f + offset).Clamp(0f, 360f), 360f + offset, degrees, Hardmode ? 6f : 3f);
                    ChurroProjectile.SpawnArc(prefabP3, input, ring);
                    iteration = iteration + 1;
                }
            }

            if (Hardmode)
            {
                handler.settings.SetSwingDuration(0.35f);
            }
            else
            {
                handler.settings.SetSwingDuration(0.6f);
            }
            Attack1(input);
            if (owner.Health <= phase3Health)
            {
                StartCoroutine(Attack2(input));
            }
        }
    }
}
