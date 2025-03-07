using Core.Extensions;
using UnityEngine;

namespace ChurroIceDungeon
{
    public class ChurroTestPierce : ChurroBaseAttack
    {
        [SerializeField] ChurroProjectile prefab;
        protected override void AttackPayload(ChurroProjectile.InputSettings input)
        {
            ChurroProjectile.SingleSettings s = new(0f, 10f);
            ChurroProjectile.SpawnSingle(prefab, input, s, out ChurroProjectile p);
            if (p != null)
            {
                p.Action_SetDamage(1500f);
            }
        }
    }
}
