using Core.Extensions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ChurroIceDungeon
{
    public class AkiAttack : ChurroBaseAttack
    {
        [SerializeField] ChurroProjectile prefab1;
        [SerializeField] ChurroProjectile prefabMentos;
        [SerializeField] float attackLength = 35f;
        protected override void AttackPayload(ChurroProjectile.InputSettings input)
        {
            IEnumerator CO_Attack()
            {
                List<ChurroProjectile> spawnIteration;
                WaitForSeconds repeatDelay = new(0.1f);
                for (int i = 0; i < 400; i++)
                {
                    Arc(90f, 270f, 112 / 8f, 2f).Spawn(new(input.Origin, Down.Rotate2D(i * 132f * 0.5f)), prefab1, out spawnIteration);
                    foreach (var item in spawnIteration)
                    {
                        item.Action_AddPosition(item.CurrentVelocity.ScaleToMagnitude(-4f));
                        item.AddEvent(new ChurroEventRotate(new(3f, 0f), 15f, 1f));
                    }

                    if (i % 3 == 0)
                    {
                        Arc(-180f, 180f, 360f / 19f, 4f).Spawn(new(input.Origin, Down.Rotate2D(i * -3)), prefab1, out spawnIteration);
                        foreach (var item in spawnIteration)
                        {
                            item.Action_AddPosition(item.CurrentVelocity.ScaleToMagnitude(-4f));
                            item.AddEvent(new ChurroEventRotate(new(3f, 0f), 15f, 1f));
                        }
                        Arc(-180f, 180f, 360f / 19f, 4f).Spawn(new(input.Origin, Down.Rotate2D(i * 2.5f)), prefab1, out spawnIteration);
                        foreach (var item in spawnIteration)
                        {
                            item.Action_AddPosition(item.CurrentVelocity.ScaleToMagnitude(-4f));
                            item.AddEvent(new ChurroEventRotate(new(3f, 0f), 15f, 1f));
                        }
                    }
                    if (i % 2 == 0)
                    {
                        Arc(-180, 180f, 360f / 8f, 3.5f).Spawn(new(input.Origin, Down.Rotate2D(i * 12f)), prefabMentos, out spawnIteration);
                        foreach (var item in spawnIteration)
                        {
                            item.Action_AddPosition(item.CurrentVelocity.ScaleToMagnitude(-4f));
                        }
                        Arc(-180, 180f, 360f / 8f, 3.5f).Spawn(new(input.Origin, Down.Rotate2D(i * -6f)), prefabMentos, out spawnIteration);
                        foreach (var item in spawnIteration)
                        {
                            item.Action_AddPosition(item.CurrentVelocity.ScaleToMagnitude(-4f));
                        }
                    }
                    attackSound.Play(input.Origin);
                    yield return repeatDelay;
                }
            }
            StartCoroutine(CO_Attack());
        }
    }
}
