using Bremsengine;
using UnityEngine;

namespace ChurroIceDungeon
{
    public class ChurroIceDemoAttack : ChurroBaseAttack
    {
        [SerializeField] ChurroProjectile arcIcicle;
        [SerializeField] ChurroProjectile bigIcicle;
        protected override void AttackPayload(ChurroProjectile.InputSettings input)
        {
            ChurroProjectile.ArcSettings arc = new(-45f, 45f, 15f * Mod_DifficultyBulletDensity, 12f * Mod_DifficultyBulletSpeed);
            ChurroProjectile.ArcSettings big = new(-2.5f, 2.5f, 5f * Mod_DifficultyBulletDensity, 12f * Mod_DifficultyBulletSpeed);

            ChurroProjectile.SpawnArc(arcIcicle, input, arc);
            ChurroProjectile.SpawnArc(bigIcicle, input, big);
        }
    }
}