using System.Collections;
using UnityEngine;
using Core.Extensions;
using Bremsengine;
using System.Collections.Generic;

namespace ChurroIceDungeon
{
    public class SaunaQuestFinal : ChurroBaseAttack
    {
        [SerializeField] ChurroProjectile spiralProjectile;
        [SerializeField] ChurroProjectile ringProjectile;
        [SerializeField] float attackLength = 35f;
        [SerializeField] int iterations = 300;
        protected override void AttackPayload(ChurroProjectile.InputSettings input)
        {
            List<ChurroProjectile> Spiral(float speed, int arms, float rotation = -90f)
            {
                return Arc(0f, 360f, 360f / (arms - 1), speed).Spawn(input.SetDirection(input.Direction.Rotate2D(rotation)), spiralProjectile);
            }
            List<ChurroProjectile> Ring(float speed, int arms, float rotation = -90f)
            {
                return Arc(0f, 360f, 360f / (arms - 1), speed).Spawn(input.SetDirection(input.Direction.Rotate2D(rotation)), ringProjectile);
            }
            handler.settings.SetSwingDuration(attackLength + 4f);
            handler.settings.SetStallDuration(attackLength);
            handler.settings.TriggerAttackTime();
            StartCoroutine(CO_Attack());
            IEnumerator CO_Attack()
            {
                float repeatDelay = attackLength / iterations;
                WaitForSeconds stall = new WaitForSeconds(repeatDelay);
                for (int i = 0; i < iterations; i++)
                {
                    input.SetDirection(DownDirection);
                    #region spirals
                    attackSound.Play(input.Origin);
                    foreach (var item in Spiral(4f, 6, -22.5f + (i * 5.7f)))
                    {
                        item.AddEvent(new ChurroEventAccelerate(new(1f, 1f), 1.5f, 6f));
                    }
                    foreach (var item in Spiral(4f, 6, -22.5f + (i * -1.9f)))
                    {
                        item.AddEvent(new ChurroEventAccelerate(new(1f, 1f), 1.5f, 6f));
                    }
                    foreach (var item in Spiral(3f, 6, 45f + (i * -1.9f)))
                    {
                        item.AddEvent(new ChurroEventAccelerate(new(1f, 1f), 1.5f, 6f));
                    }
                    foreach (var item in Spiral(3f, 6, 45f + (i * 5.2f)))
                    {
                        item.AddEvent(new ChurroEventAccelerate(new(1f, 1f), 1.5f, 6f));
                    }
                    #endregion
                    #region Rings
                    if (i % 40 == 39)
                        foreach (var item in Ring(4f, 44, Random.Range(0f, 10f)))
                        {

                        }
                    if (i % 80 == 79)
                        foreach (var item in Ring(6f, 52, Random.Range(0f, 10f)))
                        {

                        }
                    if (i % 120 == 119)
                        foreach (var item in Ring(8f, 60, Random.Range(0f, 10f)))
                        {

                        }
                    if (i % 160 == 159)
                        foreach (var item in Ring(10f, 68, Random.Range(0f, 10f)))
                        {

                        }
                    if (i % 200 == 199)
                        foreach (var item in Ring(12f, 76, Random.Range(0f, 10f)))
                        {

                        }
                    #endregion
                    yield return stall;
                }
            }
        }
    }
}
