using UnityEngine;

namespace Bremsengine
{
    public class ProjectileAttack : BaseAttack
    {
        [SerializeField] ProjectileGraphSO projectileAttack;
        public override void PerformAttack(AttackDirectionPacket packet)
        {
            ProjectileGraphInput graphDirection = new(attackHandler.OwnerTransform, packet.Target);
            if (!packet.TrackTarget) graphDirection.SetTargetStartPosition(packet.Target.position);
            graphDirection.SetOverrideDirection(packet.aimDirectionOverride);
            projectileAttack.SpawnGraph(graphDirection, OnProjectileSpawn);
        }
    }
}
