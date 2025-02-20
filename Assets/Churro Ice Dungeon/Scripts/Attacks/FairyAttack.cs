using Core.Extensions;
using UnityEngine;

namespace ChurroIceDungeon
{
    public class FairyAttack : ChurroBaseAttack
    {
        [SerializeField] ChurroProjectile projectilePrefab;
        protected override void AttackPayload(ChurroProjectile.InputSettings input)
        {
            ChurroProjectile.ArcSettings s = new(-30f, 30f, 30f, 4f);
            ChurroProjectile.SpawnArc(projectilePrefab, input, s);
        }
    }
}