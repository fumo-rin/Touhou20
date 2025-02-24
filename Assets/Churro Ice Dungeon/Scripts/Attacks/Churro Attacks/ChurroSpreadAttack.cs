using UnityEngine;

namespace ChurroIceDungeon
{
    public class ChurroSpreadAttack : ChurroBaseAttack
    {
        [SerializeField] ChurroProjectile smallArc;
        [SerializeField] ChurroProjectile frontal;
        protected override void AttackPayload(ChurroProjectile.InputSettings input)
        {
            Arc(-45f, 45f, 90f / 6f, 17f).Spawn(input, smallArc);
            Arc(-2f, 2f, 4f, 22f).Spawn(input, frontal);
        }
    }
}
