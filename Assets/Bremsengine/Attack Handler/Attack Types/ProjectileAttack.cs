using UnityEngine;

namespace Bremsengine
{
    public class ProjectileAttack : BaseAttack
    {
        [SerializeField] ProjectileGraphSO projectileAttack;
        public override void PerformAttack(AttackDirectionPacket packet)
        {
            ProjectileGraphInput graphDirection = new(attackHandler.OwnerTransform, packet.Target);
            graphDirection.SetOverrideDirection(packet.aimDirectionOverride);

            projectileAttack.SpawnGraph(graphDirection, OnProjectileSpawn);
        }
    }
}
