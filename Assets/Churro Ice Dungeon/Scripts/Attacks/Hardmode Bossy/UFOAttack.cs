using Core.Extensions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ChurroIceDungeon
{
    public class UFOAttack : ChurroBaseAttack
    {
        [SerializeField] ChurroProjectile prefab;
        protected override void AttackPayload(ChurroProjectile.InputSettings input)
        {
            input.SetOrigin(owner.CurrentPosition);
            input.SetDirection(input.Direction.Rotate2D(15f));
            ChurroProjectile.SingleSettings single = new(0f, 1.65f);

            WaitForSeconds lineStall = new(0.055f);

            IEnumerator CO_Spawn(float rotation)
            {
                ChurroProjectile.InputSettings iterationInput = new(owner.CurrentPosition, input.Direction);
                iterationInput.OnSpawn += input.OnSpawn;
                Vector2 lineStepDirection = iterationInput.Direction.Rotate2D(rotation);
                Vector2 direction = lineStepDirection;
                float randomRotation = Random.Range(1.5f, 3f);

                for (int i = 0; i < 50f; i++)
                {
                    attackSound.Play(iterationInput.Origin);
                    ChurroProjectile.SpawnSingle(prefab, iterationInput, single);
                    direction = direction.Rotate2D(Mathf.Sqrt (2f * i * 50f));
                    iterationInput.SetOrigin(iterationInput.Origin + lineStepDirection.ScaleToMagnitude(0.45f).Rotate2D(3f));
                    iterationInput.SetDirection(direction.Rotate2D(randomRotation * i));
                    yield return lineStall;
                }
                /*Vector2 lineStepDirection = (input.Direction - owner.CurrentPosition).Rotate2D(rotation);
                Vector2 offset = owner.CurrentPosition + lineStepDirection.ScaleToMagnitude(3.5f);
                float lastAttackSoundTime = Time.time;
                for (int ii = 0; ii < 35; ii++)
                {
                    iterationInput.SetOrigin(offset + (ii * lineStepDirection.ScaleToMagnitude(0.35f).Rotate2D(rotation)));
                    iterationInput.SetDirection(iterationInput.Direction.Rotate2D(2f));
                    ChurroProjectile.SpawnSingle(prefab, iterationInput, single).AddEvent(new ChurroEventAccelerate(new(3f, 0.15f), 12f, 0.9f));
                    if (Time.time > lastAttackSoundTime + 0.04f)
                    {
                        attackSound.Play(iterationInput.Origin);
                        lastAttackSoundTime = Time.time;
                    }
                    yield return lineStall;
                }
                */
            }
            for (int i = 0; i < 12; i++)
            {
                StartCoroutine(CO_Spawn(i * 30f));
            }
        }
    }
}
