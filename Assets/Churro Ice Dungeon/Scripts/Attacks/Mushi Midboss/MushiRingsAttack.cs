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
                return Arc(0f + addedRotation, 360f + addedRotation, 360f / 40, 6f).Spawn(input, prefab, out iterationList);
            }
            StartCoroutine(CO_Rings());
            IEnumerator CO_Rings()
            {
                float ultraRotation = 0;
                float addedRotation = 0f;
                for (int i = 0; i < ringCount; i++)
                {
                    input.SetOrigin(owner.CurrentPosition);
                    if (IsDifficulty(GeneralManager.Difficulty.Ultra) && i % 3 == 0 || IsDifficulty(GeneralManager.Difficulty.Lunatic) && i % 5 == 0)
                    {
                        ChurroProjectile.SingleSettings big = new(0f, 1.5f);
                        if (ChurroProjectile.SpawnSingle(bigShot, input, big, out ChurroProjectile output))
                        {
                            ChurroEventAccelerate speedup = new(new(1.5f, 0.35f), 8f, 6f);
                            output.Action_AddPosition(output.CurrentVelocity.ScaleToMagnitude(0.5f));
                            output.Action_SetSpriteLayerIndex(0);
                            output.AddEvent(speedup);
                        }
                    }
                    if (IsDifficulty(GeneralManager.Difficulty.Ultra))
                    {
                        ultraRotation += ringRandom.Spread(70f).RandomPositiveNegativeRange().Multiply((i - 1).Max(0));
                        TrySpawnRing(-ultraRotation);
                        yield return ringRepeatStall;
                    }
                    if (IsDifficulty(GeneralManager.Difficulty.Lunatic))
                    {
                        TrySpawnRing(addedRotation);
                        yield return ringRepeatStall;
                    }
                    TrySpawnRing(addedRotation);
                    addedRotation += ringRandom.Spread(70f).RandomPositiveNegativeRange().Multiply((i - 1).Max(0));
                    yield return ringRepeatStall;
                }
            }
        }
    }
}
