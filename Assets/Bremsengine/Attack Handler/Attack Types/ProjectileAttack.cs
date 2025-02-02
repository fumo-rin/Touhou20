using UnityEngine;

namespace Bremsengine
{
    public class ProjectileAttack : BaseAttack
    {
        [SerializeField] ProjectileGraphSO projectileAttack;
        [SerializeField] int projectileSortingIndex;
        public void SetAttackGraph(ProjectileGraphSO newGraph) => projectileAttack = newGraph;
        public override float GetAttackCooldown()
        {
            if (projectileAttack == null)
            {
                return 0.1f;
            }
            return projectileAttack.CalculateCooldown(0f);
        }

        public override void PerformAttack(AttackDirectionPacket packet)
        {
            if (projectileAttack == null)
            {
                return;
            }
            ProjectileGraphInput graphDirection = new(attackHandler.OwnerTransform, packet.Target);
            if (packet.Target != null) graphDirection.SetTargetStartPosition(packet.Target.position);
            graphDirection.SetOverrideDirection(packet.aimDirectionOverride);
            projectileAttack.SpawnGraph(graphDirection, OnProjectileSpawn, projectileSortingIndex);
        }
    }
}
