using Core.Extensions;
using UnityEngine;

namespace ChurroIceDungeon
{
    public class FairyAttack : ChurroBaseAttack
    {
        [SerializeField] ChurroProjectile projectilePrefab;
        protected override void AttackPayload(ChurroProjectile.InputSettings input)
        {
            ChurroProjectile.ArcSettings s = new(-40f, 40f, 40f, 4f);
            if (Hardmode)
            {
                s.Widen(2f);
                s = s * 2f;
            }
            ChurroProjectile.SpawnArc(projectilePrefab, input, s);
        }
    }
}