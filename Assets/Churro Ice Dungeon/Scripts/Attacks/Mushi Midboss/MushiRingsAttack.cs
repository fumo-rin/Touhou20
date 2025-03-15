using Bremsengine;
using Core.Extensions;
using System.Collections;
using UnityEngine;

namespace ChurroIceDungeon
{
    public class MushiRingsAttack : ChurroBaseAttack
    {
        [SerializeField] int ringCount = 40;
        [SerializeField] float ringRandom = 1f;
        [SerializeField] float ringDensity = 20f;
        [SerializeField] ChurroProjectile prefab;
        [SerializeField] ChurroProjectile bigShot;
        protected override void AttackPayload(ChurroProjectile.InputSettings input)
        {
            WaitForSeconds ringRepeatStall = new WaitForSeconds(handler.settings.StallDuration / ringCount);
            if (IsDifficulty(GeneralManager.Difficulty.Ultra) || IsDifficulty(GeneralManager.Difficulty.Lunatic))
            {
                ringRepeatStall = new WaitForSeconds(handler.settings.StallDuration / ringCount.MultiplyAndFloorAsFloat(2f));
            }
            bool TrySpawnRing(float addedRotation)
            {
                bool success = Arc(0f + addedRotation, 360f + addedRotation, 360f / 28, 12f).Spawn(input, prefab, out iterationList);
                foreach (var item in iterationList)
                {

                }
                return success;
            }
            StartCoroutine(CO_Rings());
            if (IsDifficulty(GeneralManager.Difficulty.Ultra))
            {
                StartCoroutine(CO_Balls(0.4f, 10));
            }
            if (IsDifficulty(GeneralManager.Difficulty.Lunatic))
            {
                StartCoroutine(CO_Balls(0.7f, 6));
            }
            IEnumerator CO_Balls(float interval, int count)
            {
                for (int i = 0; i < count; i++)
                {
                    ChurroProjectile.SingleSettings big = new(0f, 1.5f);
                    if (ChurroProjectile.SpawnSingle(bigShot, input, big, out ChurroProjectile output))
                    {
                        output.Action_AddPosition(output.CurrentVelocity.ScaleToMagnitude(0.5f));
                        output.Action_SetSpriteLayerIndex(0);
                        output.AddEvent(new ChurroEventAccelerate(new(1.5f, 0.05f), 8f, 3f));
                        output.AddOnScreenExitEvent((ChurroProjectile p, Vector2 normal) => p.Action_SetVelocity(p.CurrentVelocity.Bounce(normal, 5f), p.CurrentVelocity.magnitude));
                        if (input.OptionalTarget != null)
                        {
                            output.Action_Retarget(input.OptionalTarget);
                        }
                        yield return new WaitForSeconds(interval);
                    }
                }
            }
            IEnumerator CO_Rings()
            {
                float ultraRotation = 0;
                float addedRotation = 0f;
                for (int i = 0; i < ringCount; i++)
                {
                    input.SetOrigin(owner.CurrentPosition);
                    if (IsDifficulty(GeneralManager.Difficulty.Ultra))
                    {
                        ultraRotation += 0.5f + ringRandom.Spread(70f).Multiply((i - 1).Max(0));
                        TrySpawnRing(-ultraRotation);
                        foreach (var item in iterationList)
                        {
                            item.Action_AddPosition(item.CurrentVelocity.ScaleToMagnitude(3f));
                            item.Action_AddRotation(180f + 35f);
                            item.AddEvent(new ChurroEventAccelerate(new(2f, 0.25f), 3.25f, 14f));
                        }
                        yield return ringRepeatStall;
                    }
                    if (IsDifficulty(GeneralManager.Difficulty.Lunatic))
                    {
                        TrySpawnRing(addedRotation);
                        foreach (var item in iterationList)
                        {
                            item.Action_AddPosition(item.CurrentVelocity.ScaleToMagnitude(3f));
                            item.Action_AddRotation(180f);
                            item.AddEvent(new ChurroEventAccelerate(new(2f, 0.25f), 3.25f, 14f));
                        }
                        yield return ringRepeatStall;
                    }
                    TrySpawnRing(addedRotation);
                    foreach (var item in iterationList)
                    {
                        item.Action_AddPosition(item.CurrentVelocity.ScaleToMagnitude(3f));
                        item.Action_AddRotation(180f - 15f);
                        item.AddEvent(new ChurroEventAccelerate(new(2f, 0.25f), 3.25f, 14f));
                    }
                    addedRotation += ringRandom.Spread(70f).Multiply((i - 1).Max(0));
                    yield return ringRepeatStall;
                }
            }
        }
    }
}
