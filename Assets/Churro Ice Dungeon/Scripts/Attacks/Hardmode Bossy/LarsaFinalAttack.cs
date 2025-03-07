using Core.Extensions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ChurroIceDungeon
{
    public class LarsaFinalAttack : ChurroBaseAttack
    {
        [SerializeField] ChurroProjectile spiralProjectile;
        [SerializeField] ChurroProjectile secondaryProjectile;
        [SerializeField] AudioClipWrapper spiralSound;
        protected override void AttackPayload(ChurroProjectile.InputSettings input)
        {
            List<ChurroProjectile> Spiral(int arms, float rotation = -90f)
            {
                Arc(-135f, 135f, 270f / (arms -1), 6f).Spawn(input.SetDirection(input.Direction.Rotate2D(rotation)), spiralProjectile, out iterationList);
                return iterationList;
            }
            List<ChurroProjectile> Secondary(int arms, float rotation = -180f)
            {
                Arc(-180f, 180f, 360f / (arms - 1), 3.5f).Spawn(input.SetDirection(input.Direction.Rotate2D(rotation)), secondaryProjectile, out iterationList);
                return iterationList;
            }
            void ApplySpiral1Events(List<ChurroProjectile> p)
            {
                ChurroEventRotate rotation = new(new(0.75f, 0.5f), 45f, 4f);
                ChurroEventRotate rotation2 = new(new(1.25f, 1.25f), -75f, 2.5f);
                ChurroEventAccelerate slowdown = new(new(2f, 0.25f), 2f, 6f);
                foreach (var item in p)
                {
                    item.AddEvent(slowdown);
                    item.AddEvent(rotation);
                    item.AddEvent(rotation2);
                }
            }
            void ApplySpiral2Events(List<ChurroProjectile> p)
            {
                ChurroEventRotate rotation = new(new(1.5f, 0.5f), -45f, 4f);
                ChurroEventAccelerate slowdown = new(new(2f, 0.25f), 2f, 6f);
                foreach (var item in p)
                {
                    item.AddEvent(slowdown);
                    item.AddEvent(rotation);
                }
            }
            IEnumerator CO_Secondary()
            {
                List<ChurroProjectile> iteration;
                WaitForSeconds repeatDelay = new(0.35f);
                for (float i = 0; i < 25; i++)
                {
                    Debug.Log(i);
                    attackSound.Play(owner.CurrentPosition);
                    input.SetOrigin(owner.CurrentPosition);
                    iteration = Secondary(5, -180f + (i.Squared() * 12f));
                    foreach (var item in iteration)
                    {
                        item.AddEvent(new ChurroEventRotate(new(1.5f, 0.1f), 160f, 6f));
                        item.AddEvent(new ChurroEventRotate(new(2f, 1.6f), -90f, 8f));
                    }
                    yield return repeatDelay;
                }
            }
            IEnumerator CO_Attack()
            {
                List<ChurroProjectile> iteration;
                WaitForSeconds repeatDelay = new(0.15f);
                for (int i = 0; i < 68; i++)
                {
                    spiralSound.Play(owner.CurrentPosition);
                    input.SetOrigin(owner.CurrentPosition);
                    iteration = Spiral(14, -90f + (i * 0.7f));
                    ApplySpiral1Events(iteration);
                    iteration = Spiral(24, -90f + (i * -1.1f));
                    ApplySpiral2Events(iteration);
                    yield return repeatDelay;
                }
            }
            StartCoroutine(CO_Attack());
            StartCoroutine(CO_Secondary());
        }
    }
}
