using UnityEngine;

namespace ChurroIceDungeon
{
    public class BowapAttack : ChurroBaseAttack
    {
        float spin = 0f;
        [SerializeField] float spinIncrement = 1f;
        int spinDex = 1;
        [SerializeField] ChurroProjectile prefab;
        protected override void AttackPayload(ChurroProjectile.InputSettings input)
        {
            spin += (spinIncrement * spinDex) % 360f;
            spinDex++;
            ChurroProjectile.ArcSettings bowap = new(0f + spin, 360f + spin, 72f, 7f);
            ChurroProjectile.SpawnArc(prefab, input, bowap);
        }
    }
}
