using Core.Extensions;
using Mono.CSharp;
using System.Collections;
using UnityEngine;

namespace ChurroIceDungeon
{
    public class MushiFrontals : ChurroBaseAttack
    {
        [SerializeField] Transform targetRef;
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
                    sideInput.SetDirection((Vector2)targetRef.position - (Vector2)shot.position);
                    sideInput.SetDirection(sideInput.Direction.Rotate2D(rotation));
                    var side = Arc(-65f + 6.5f, 65 + 6.5f, 130 / 6f, 11f).Widen(widen);
                    side.Spawn(sideInput, sideshotPrefab);
                    attackSound.Play(sideInput.Origin);
                }
                float timeBetweenShots = duration / shots;
                WaitForSeconds stall = new WaitForSeconds(timeBetweenShots);
                float time = 0f;
                float widen = 1f;
                float widenMultiplier = 0.985f;
                for (float i = 0; i < shots; i++)
                {
                    time += timeBetweenShots;
                    float rotation = 4.5f * Mathf.Sin(time * 0.43f);
                    widen *= rotation.Sign() < 0 ? (1f / widenMultiplier) : widenMultiplier;
                    SideShot(leftShot, rotation, widen);
                    SideShot(rightshot, rotation, widen);
                    if (i.Floor().ToInt() % frontalEvery == 0)
                    {
                        bigShotSound.Play(input.Origin);
                        foreach (var item in Arc(-15f, 15f, 30f / 3f, 6f).Spawn(input, frontalPrefab))
                        {
                            item.Action_AddRotation(3f.Spread(100f));
                            item.Action_MultiplyVelocity(1f.Spread(15f));
                        }
                    }
                    bool flip = false;
                    if (i.Floor().ToInt() % ringEvery == 0)
                    {
                        bigShotSound.Play(input.Origin);
                        foreach (var item in Arc(0f, 360f, 360f / 20f, 6f).Spawn(input, ringPrefab))
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
