using Core.Extensions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ChurroIceDungeon
{
    public class YukariAttack : ChurroBaseAttack
    {

        [SerializeField] DungeonUnit attackOwner;
        //Works Pretty well with 0.03 Fire Rate and -0.3f Spincrement
        float spin = 20f;
        float spinIncrement => -(timePerShot) * 10f;
        int spinDex = -35;
        [SerializeField] ChurroProjectile bowapProjectile;
        Coroutine currentAttack;
        [SerializeField] float attackLength = 32f;
        [SerializeField] float timePerShot = 0.04f;
        protected override void AttackPayload(ChurroProjectile.InputSettings input)
        {
            WaitForSeconds bowapStall = new(timePerShot * (Hardmode ? 0.8f : 1f));
            List<ChurroProjectile> spawnIteration = new();
            IEnumerator CO_Yukari()
            {
                if (handler != null)
                {
                    handler.settings.SetStallDuration(attackLength + 2f);
                    handler.settings.SetSwingDuration(attackLength + 6f);
                    handler.settings.TriggerAttackTime();
                    if (attackOwner is EnemyUnit e)
                    {
                        e.SetStallTime(handler.settings.StallDuration);
                    }
                }
                float speedMod = Hardmode ? 0.50f : 0.35f;
                float elapsedTime = 0f;
                while (attackOwner.IsAlive() && elapsedTime < attackLength)
                {
                    elapsedTime += timePerShot;
                    speedMod = speedMod.MoveTowards(1f, timePerShot * 0.3f);
                    spin += (spinIncrement.Multiply(Hardmode ? 0.8f : 1f) * spinDex);
                    //spin += Mathf.Sin(Time.deltaTime * 30f) + -3f;
                    spin = spin % 360f;
                    spinDex++;
                    ChurroProjectile.ArcSettings bowap = new(0f + spin, 360f + spin, 360f / 5f, 5f * speedMod * (elapsedTime * 1f).Clamp(1, Hardmode ? 1.4f : 1.2f));
                    if (!ChurroManager.HardMode)
                    {
                        bowap = bowap * 0.666f;
                        bowap = bowap.Speed(0.5f);
                    }

                    input.SetOrigin(attackOwner.CurrentPosition);
                    spawnIteration = ChurroProjectile.SpawnArc(bowapProjectile, input, bowap);
                    foreach (ChurroProjectile p in spawnIteration)
                    {
                        p.Action_SetFaction(attackOwner.Faction);
                        p.Action_AddPosition(p.CurrentVelocity.ScaleToMagnitude(0.65f));
                    }
                    attackSound.Play(transform.position);
                    yield return bowapStall;
                }
                currentAttack = null;
            }
            if (currentAttack != null)
            {
                StopCoroutine(currentAttack);
            }
            currentAttack = StartCoroutine(CO_Yukari());
        }
    }
}
