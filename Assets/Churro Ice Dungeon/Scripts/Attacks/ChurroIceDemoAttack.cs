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
            ChurroProjectile.ArcSettings arc = new(-45f, 45f, 15f, 16f);
            ChurroProjectile.ArcSettings big = new(-2.5f, 2.5f, 5f, 16f);

            ChurroProjectile.SpawnArc(arcIcicle, input, arc);
            ChurroProjectile.SpawnArc(bigIcicle, input, big);
        }
    }
}