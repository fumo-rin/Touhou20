using UnityEngine;

namespace ChurroIceDungeon
{
    public class FairyRevenge : ChurroBaseAttack
    {
        [SerializeField] ChurroProjectile prefab;
        protected override void AttackPayload(ChurroProjectile.InputSettings input)
        {
            ChurroProjectile.ArcSettings arc = new(0f, 360f, Hardmode ? 45f : 120f, Hardmode ? 3.5f : 2f);
            ChurroProjectile.SpawnArc(prefab, input, arc, out _);
        }
    }
}
