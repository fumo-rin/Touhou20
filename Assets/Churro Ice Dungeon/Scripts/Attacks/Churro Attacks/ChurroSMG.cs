using Core.Extensions;
using UnityEngine;

namespace ChurroIceDungeon
{
    public class ChurroSMG : ChurroBaseAttack
    {
        [SerializeField] ChurroProjectile prefab;
        protected override void AttackPayload(ChurroProjectile.InputSettings input)
        {
            ChurroProjectile.ArcSettings arc = new(Random.Range(-4f, -1.55f), Random.Range(1.55f, 4f), 2.5f, 27f);
            foreach (var item in arc.Spawn(input, prefab))
            {
                item.Action_MultiplyVelocity(1f.Spread(10f));
            }
        }
    }
}
