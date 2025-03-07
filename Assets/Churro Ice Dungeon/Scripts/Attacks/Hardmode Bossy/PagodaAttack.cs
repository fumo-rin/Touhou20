using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Core.Extensions;


namespace ChurroIceDungeon
{
    public class PagodaAttack : ChurroBaseAttack
    {
        [SerializeField] ChurroProjectile prefab;
        protected override void AttackPayload(ChurroProjectile.InputSettings input)
        {
            input.SetOrigin(owner.CurrentPosition);
            input.SetDirection(input.Direction.Rotate2D(30f));
            ChurroProjectile.SingleSettings single = new(0f, 0.65f);

            WaitForSeconds lineStall = new(0.005f);

            IEnumerator CO_Spawn(float rotation)
            {
                ChurroProjectile.InputSettings iterationInput = new(owner.CurrentPosition, input.Direction);
                iterationInput.OnSpawn += input.OnSpawn;
                Vector2 lineStepDirection = iterationInput.Direction.Rotate2D(rotation);
                Vector2 direction = lineStepDirection.Rotate2D(Random.Range(-15f, 15f));
                float lastAttackSoundTime = Time.time;

                for (int i = 0; i < 60f; i++)
                {
                    if (ChurroProjectile.SpawnSingle(prefab, iterationInput, single, out iterationProjectile))
                    {
                        iterationProjectile.AddEvent(new ChurroEventAccelerate(new(5f, 0.5f), 16f, 2.5f)).AddEvent(new ChurroEventAccelerate(new(0.45f, 0.05f), 6f, 0.8f));
                    }
                    direction = direction.Rotate2D(1.5f * i);
                    iterationInput.SetOrigin(iterationInput.Origin + lineStepDirection.ScaleToMagnitude(0.28f));
                    iterationInput.SetDirection(direction);
                    if (Time.time > lastAttackSoundTime + 0.03f)
                    {
                        attackSound.Play(iterationInput.Origin);
                        lastAttackSoundTime = Time.time;
                    }
                    yield return lineStall;
                }
            }
            for (int i = 0; i < 6; i++)
            {
                StartCoroutine(CO_Spawn(i * 60f));
                StartCoroutine(CO_Spawn(3f + i * 60f));
            }
        }
    }
}
