using Core.Extensions;
using Mono.CSharp;
using System.Collections;
using UnityEngine;

namespace ChurroIceDungeon
{
    public class MushiFrontals : ChurroBaseAttack
    {
        [SerializeField] Transform leftShot;
        [SerializeField] Transform rightshot;
        [SerializeField] float duration = 25f;
        [SerializeField] float shots = 350;
        [SerializeField] ChurroProjectile sideshotPrefab;
        [SerializeField] int frontalEvery = 35;
        [SerializeField] ChurroProjectile frontalPrefab;
        [SerializeField] AudioClipWrapper bigShotSound;
        [SerializeField] int ringEvery = 85;
        [SerializeField] ChurroProjectile ringPrefab;
        [SerializeField] ChurroProjectile ultraPrefab;
        protected override void AttackPayload(ChurroProjectile.InputSettings input)
        {
            handler.settings.SetStallDuration(1f);
            handler.settings.SetSwingDuration(duration + 1);
            IEnumerator CO_Attack()
            {
                ChurroProjectile.InputSettings sideInput = new(input.Origin, input.Direction);
                void SideShot(Transform shot, float rotation, float widen)
                {
                    sideInput.OnSpawn += input.OnSpawn;
                    sideInput.SetOrigin(shot.position);
                    sideInput.SetDirection((Vector2)input.OptionalTarget.position - (Vector2)shot.position);
                    sideInput.SetDirection(sideInput.Direction.Rotate2D(rotation));
                    var side = Arc(-75f, 75, 150f / 7f, 9f).Widen(widen);
                    side.Spawn(sideInput, sideshotPrefab, out _);
                    attackSound.Play(sideInput.Origin);
                }
                float timeBetweenShots = duration / shots;
                WaitForSeconds stall = new WaitForSeconds(timeBetweenShots);
                float time = 0f;
                float widen = 1f;
                float widenMultiplier = 0.9935f;
                for (float i = 0; i < shots; i++)
                {
                    time += timeBetweenShots;
                    float rotation = (IsDifficulty(Bremsengine.GeneralManager.Difficulty.Ultra) ? 6.5f : 4.5f) * Mathf.Sin(time * 0.43f);
                    widen *= rotation.Sign() < 0 ? (1f / widenMultiplier) : widenMultiplier;
                    SideShot(leftShot, rotation, widen);
                    SideShot(rightshot, rotation, widen);
                    if (i.Floor().ToInt() % frontalEvery == 0)
                    {
                        input.SetOrigin(owner.CurrentPosition);
                        if (input.OptionalTarget != null)
                        {
                            input.SetDirection((Vector2)input.OptionalTarget.position - input.Origin);
                        }
                        bigShotSound.Play(input.Origin);
                        Arc(-25f, 25f, 50f / 8f, 2f).Spawn(input, frontalPrefab, out iterationList);
                        foreach (var item in iterationList)
                        {
                            item.Action_AddRotation(3f.Spread(100f));
                            item.Action_MultiplyVelocity(1f.Spread(15f));
                            item.Action_SetSpriteLayerIndex(200);
                            if (IsDifficulty(Bremsengine.GeneralManager.Difficulty.Ultra))
                            {
                                item.AddEvent(new ChurroEventAccelerate(new(1.5f, 0.25f), 9f, 8f));
                            }
                            else
                            {
                                item.AddEvent(new ChurroEventAccelerate(new(1.5f, 0.25f), 6f, 5f));
                            }
                        }
                    }
                    bool flip = false;
                    if (i.Floor().ToInt() % (IsDifficulty(Bremsengine.GeneralManager.Difficulty.Ultra) ? (ringEvery / 10f).ToInt() : ringEvery) == 0)
                    {
                        input.SetOrigin(owner.CurrentPosition);
                        if (input.OptionalTarget != null)
                        {
                            input.SetDirection((Vector2)input.OptionalTarget.position - input.Origin);
                        }
                        bigShotSound.Play(input.Origin);
                        Arc(0f, 360f, 360f / 20f, IsDifficulty(Bremsengine.GeneralManager.Difficulty.Ultra) ? 1.5f : 7.5f).Spawn(input, IsDifficulty(Bremsengine.GeneralManager.Difficulty.Ultra) ? ultraPrefab : ringPrefab, out iterationList);
                        foreach (var item in iterationList)
                        {
                            item.Action_AddRotation(360f / 40f);
                            item.AddEvent(new ChurroEventRotate(new(2f, 0.5f), flip.AsFloat(1f, -1f) * 25f, 6f));
                            flip = !flip;
                        }
                    }

                    yield return stall;
                }
            }
            StartCoroutine(CO_Attack());
        }
    }
}
