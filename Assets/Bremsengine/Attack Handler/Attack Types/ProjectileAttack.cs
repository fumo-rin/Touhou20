using UnityEngine;

namespace Bremsengine
{
    public class ProjectileAttack : BaseAttack
    {
        [SerializeField] ProjectileGraphSO projectileAttack;

        public override float GetAttackCooldown()
        {
            return projectileAttack.CalculateCooldown(0f);
        }

        public override void PerformAttack(AttackDirectionPacket packet)
        {
            ProjectileGraphInput graphDirection = new(attackHandler.OwnerTransform, packet.Target);
            if (packet.Target != null) graphDirection.SetTargetStartPosition(packet.Target.position);
            graphDirection.SetOverrideDirection(packet.aimDirectionOverride);
            projectileAttack.SpawnGraph(graphDirection, OnProjectileSpawn);
        }
    }
}
