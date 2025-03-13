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
            ChurroProjectile.ArcSettings s = new(-40f, 40f, 40f, projectileSpeed * 3f);
            if (Hardmode)
            {
                s.Widen(2f);
                s = s * 2f;
            }
            ChurroProjectile.SpawnArc(projectilePrefab, input, s, out iterationList);
            foreach (var iteration in iterationList)
            {
                ChurroEventAccelerate accelerate = new(new(1f, 0.05f), projectileSpeed, 30f);
                iteration.Action_AddPosition(iteration.CurrentVelocity.ScaleToMagnitude(0.75f));
                iteration.AddEvent(accelerate);
            }
        }
    }
}