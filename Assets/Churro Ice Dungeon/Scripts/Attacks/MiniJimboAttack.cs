using Core.Extensions;
using System.Collections;
using UnityEngine;

namespace ChurroIceDungeon
{
    public class MiniJimboAttack : ChurroBaseAttack
    {
        [SerializeField] ChurroProjectile prefab;
        protected override void AttackPayload(ChurroProjectile.InputSettings input)
        {
            float speed = Hardmode ? 4f : 3f;
            StartCoroutine(CO_Lines(speed));
            ChurroProjectile.SingleSettings single = new(0f, speed);
            for (int i = 0; i < 7; i++)
            {
                single.ProjectileSpeed = speed;
                ChurroProjectile.SpawnSingle(prefab, input, single).Action_SetFaction(Bremsengine.BremseFaction.Enemy);
                speed *= 1.15f;
            }
            IEnumerator CO_Lines(float speed)
            {
                if (!Hardmode)
                    yield break;
                for (int repeats = 0; repeats < 3; repeats++)
                {
                    if (owner == null || !owner.IsAlive())
                        yield break;
                    ChurroProjectile.ArcSettings arc = new(-7f * repeats + 1, 7f * repeats + 2, 14f * repeats +1, speed);
                    yield return new WaitForSeconds(0.075f);
                    for (int i = 0; i < 4; i++)
                    {
                        foreach (var item in ChurroProjectile.SpawnArc(prefab, input, arc))
                        {
                            item.Action_SetFaction(Bremsengine.BremseFaction.Enemy);
                        }
                        arc = arc.Speed(1.25f);
                    }
                    attackSound.Play(owner.CurrentPosition);
                }
            }
        }
    }
}
