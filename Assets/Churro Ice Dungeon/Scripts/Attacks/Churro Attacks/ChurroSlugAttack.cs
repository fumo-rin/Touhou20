using Bremsengine;
using UnityEngine;
using Core.Extensions;

namespace ChurroIceDungeon
{
    public class ChurroSlugAttack : ChurroBaseAttack
    {
        [SerializeField] ChurroProjectile arcIcicle;
        [SerializeField] ChurroProjectile bigIcicle;
        [SerializeField] int arcCount = 10;
        [SerializeField] float arcAngle = 8f;
        protected override void OnAttackLoad()
        {
            base.OnAttackLoad();
        }
        protected override void AttackPayload(ChurroProjectile.InputSettings input)
        {
            ChurroProjectile.ArcSettings arc = new(-arcCount.MultiplyAndFloorAsFloat(0.51f * arcAngle), arcCount.MultiplyAndFloorAsFloat(0.51f * arcAngle), arcAngle, 16f);
            ChurroProjectile.ArcSettings big = new(-4f, 4f, 8f/6f, 21f);
            ChurroProjectile.SpawnArc(bigIcicle, input, big);

            foreach (var item in ChurroProjectile.SpawnArc(arcIcicle, input, arc))
            {

            }
        }
    }
}