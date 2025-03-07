using Bremsengine;
using UnityEngine;
using Core.Extensions;
using System.Collections.Generic;

namespace ChurroIceDungeon
{
    public class ChurroSlugAttack : ChurroBaseAttack
    {
        [SerializeField] ChurroProjectile arcIcicle;
        [SerializeField] ChurroProjectile bigIcicle;
        [SerializeField] int arcCount = 10;
        [SerializeField] float arcAngle = 8f;
        protected override void AttackPayload(ChurroProjectile.InputSettings input)
        {
            Arc(-4f, 4f, 8f / 6f, 21f).Spawn(input, bigIcicle, out _);
            Arc(-arcCount.MultiplyAndFloorAsFloat(0.51f * arcAngle), arcCount.MultiplyAndFloorAsFloat(0.51f * arcAngle), arcAngle, 16f).Spawn(input, arcIcicle, out _);
        }
    }
}