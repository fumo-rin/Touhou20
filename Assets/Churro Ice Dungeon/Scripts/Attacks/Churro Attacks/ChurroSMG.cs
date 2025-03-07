using Core.Extensions;
using System.Collections.Generic;
using UnityEngine;

namespace ChurroIceDungeon
{
    public class ChurroSMG : ChurroBaseAttack
    {
        [SerializeField] ChurroProjectile prefab;
        protected override void AttackPayload(ChurroProjectile.InputSettings input)
        {
            ChurroProjectile.ArcSettings arc = new(Random.Range(-4f, -1.55f), Random.Range(1.55f, 4f), 2.5f, 27f);
            arc.Spawn(input, prefab, out List<ChurroProjectile> output);
            foreach (var item in output)
            {
                item.Action_MultiplyVelocity(1f.Spread(10f));
            }
        }
    }
}
