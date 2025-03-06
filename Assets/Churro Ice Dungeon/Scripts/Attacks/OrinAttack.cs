using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using Core.Extensions;
using Bremsengine;
using UnityEngine.Tilemaps;

namespace ChurroIceDungeon
{
    public class OrinAttack : ChurroBaseAttack
    {
        [SerializeField] GameObject humanRig;
        [SerializeField] GameObject catRig;
        [SerializeField] DungeonMotor whenMoveMotor;
        [SerializeField] ChurroProjectile catwalkProjectile;
        [SerializeField] float speedDelay;
        int ringsLunatic = 4;
        int ringsNormal = 2;
        [SerializeField] float projectileSpeed = 4f;
        [SerializeField] float acceleration = 1f;
        [SerializeField] int ringCount = 12;
        [SerializeField] float interval = 0.5f;
        [Header("Fairies")]
        [SerializeField] DungeonUnit prefab;

        protected override void AttackPayload(ChurroProjectile.InputSettings input)
        {
            WaitForSeconds expandDelay = new WaitForSeconds(speedDelay.Multiply(Hardmode ? 1.75f : 1f));
            float ringRepeatTime = 0.1f;
            WaitForSeconds ringRepeat = new WaitForSeconds(ringRepeatTime);
            float ringstepAngle = 45f;
            float interval = this.interval * (Hardmode ? 0.7f : 1f);
            handler.settings.SetStallDuration(0.05f);
            owner.Action_SpeedmodTowards(0.5f * interval, 10f);
            handler.settings.SetSwingDuration(4f + ringCount * interval);
            SetRig(catRig);

            for (int i = 0; i < (Hardmode ? 3 : 1); i++)
            {
                StartCoroutine(CO_SpawnFairies(input.Origin + input.Direction));
            }
            void SetRig(GameObject rig)
            {
                catRig.SetActive(false);
                humanRig.SetActive(false);
                rig.SetActive(true);
            }
            IEnumerator CO_SpawnFairies(Vector2 start)
            {
                int amount = 3;
                Vector2 Jump(Vector2 v, float maxDistance = 2f)
                {
                    return v + Random.insideUnitCircle.ScaleToMagnitude(maxDistance.RandomPositiveNegativeRange());
                }
                bool navmeshHit = false;
                Vector2 target = input.Origin + input.Direction;
                Vector2 iteration = (target) + Random.insideUnitCircle.ScaleToMagnitude(5f.Spread(15f));
                float smallestDistanceToPlayer = 4f.Squared();
                for (int i = 0; i < (amount); i++)
                {
                    for (int navmesh = 0; navmesh < 15; navmesh++)
                    {
                        iteration = Jump(iteration, 2f.Spread(25f));
                        if (owner.NavmeshContains(iteration) && iteration.SquareDistanceTo(target) > smallestDistanceToPlayer)
                        {
                            navmeshHit = true;
                            smallestDistanceToPlayer = iteration.SquareDistanceTo(target);
                        }
                    }
                    if (navmeshHit)
                    {
                        DungeonUnit unit = Instantiate(prefab, iteration, Quaternion.identity);
                        yield return new WaitForSeconds(Hardmode ? 0.15f : 0.25f);
                    }
                }
            }
            IEnumerator CO_Catwalk(ChurroProjectile projectile)
            {
                float projectileSpeed = Hardmode ? this.projectileSpeed * 1.35f : this.projectileSpeed;
                Vector2 storedSpeed = projectile.CurrentVelocity;
                float velocity = 0f;
                projectile.Action_SetVelocity(storedSpeed, 0.05f);
                yield return expandDelay;
                while (projectile.CurrentVelocity.sqrMagnitude < projectileSpeed.Squared())
                {
                    velocity += (0.5f + velocity.Squared()) * Time.deltaTime * acceleration.Multiply(Hardmode ? 1f : 0.5f);
                    projectile.Action_SetVelocity(storedSpeed, velocity);
                    yield return null;
                }
            }
            IEnumerator CO_SpawnRings(float startingDistance, float stepDistance)
            {
                int rings = Hardmode ? ringsLunatic : ringsNormal;
                for (int ii = 0; ii < ringCount; ii++)
                {
                    float offset = Random.Range(0f, ringstepAngle);
                    input.SetOrigin(owner.CurrentPosition);
                    float ringSubtractiveTime = 0f;
                    for (int i = 0; i < rings; i++)
                    {
                        offset += ringstepAngle.Multiply(0.5f.RandomPositiveNegativeRange());
                        if (owner == null || !owner.IsAlive())
                        {
                            yield break;
                        }
                        ChurroProjectile.ArcSettings ring = new((0f + offset).Clamp(0f, 360f), 360f + offset, ringstepAngle, projectileSpeed);
                        List<ChurroProjectile> spawnedRing = ChurroProjectile.SpawnArc(catwalkProjectile, input, ring);

                        foreach (var item in spawnedRing)
                        {
                            item.Action_AddPosition(item.CurrentVelocity.ScaleToMagnitude(startingDistance + (i * stepDistance)));
                            item.StartCoroutine(CO_Catwalk(item));
                        }
                        attackSound.Play(input.Origin);
                        ringSubtractiveTime += ringRepeatTime;
                        yield return ringRepeat;
                    }
                    yield return new WaitForSeconds((interval - ringSubtractiveTime).Max(0f));
                }
                SetRig(humanRig);

            }
            StartCoroutine(CO_SpawnRings(1.5f, 0.35f));
        }
    }
}
