using Core.Extensions;
using UnityEngine;

namespace ChurroIceDungeon
{
    public class FairyAttack : ChurroBaseAttack
    {
        [SerializeField] ChurroProjectile projectilePrefab;
        protected override void AttackPayload(ChurroProjectile.InputSettings input)
        {
            ChurroProjectile.ArcSettings s = new(-30f, 30f, 30f * Mod_DifficultyBulletDensity.Min(2f), 1.25f * Mod_DifficultyBulletSpeed);
            ChurroProjectile.SpawnArc(projectilePrefab, input, s);
        }
    }
}