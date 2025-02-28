using Core.Extensions;
using UnityEngine;

namespace ChurroIceDungeon
{
    public class FairyAttack : ChurroBaseAttack
    {
        [SerializeField] ChurroProjectile projectilePrefab;
        [SerializeField] float projectileSpeed = 4f;
        protected override void AttackPayload(ChurroProjectile.InputSettings input)
        {
            ChurroProjectile.ArcSettings s = new(-40f, 40f, 40f, projectileSpeed);
            if (Hardmode)
            {
                s.Widen(2f);
                s = s * 2f;
            }
            ChurroProjectile.SpawnArc(projectilePrefab, input, s);
        }
    }
}