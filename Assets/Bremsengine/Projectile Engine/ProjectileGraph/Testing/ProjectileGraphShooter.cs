using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Bremsengine
{
    
    /// <summary>
    /// You should make additions outside the scope of Bremsengine, so you can easily put your own extension on top.
    /// 
    /// for example a damage Modifier setup through items / buffs could easily be done in OnProjectileSpawn.
    /// it wouldn't work inside Bremsengine scope, since you shouldn't put stuff in here to keep it as bare bones as it should be.
    /// 
    /// </summary>
    public class ProjectileGraphShooter : MonoBehaviour
    {
        [SerializeField] ProjectileGraphSO graph;
        [SerializeField] Transform testTarget;
        [SerializeField] Transform owner;
        [SerializeField] BremseFaction faction;
        Transform Target => testTarget;
        [SerializeField] Vector2 fallbackDirection = Vector2.down;
        Vector2 OverridePosition => Target == null ? fallbackDirection : Target.position;
        public void ShootCurrentGraph()
        {
            if (graph == null)
            {
                Debug.Log("Missing Projectile attack graph");
                return;
            }
            ProjectileGraphInput input = new(owner, Target);

            input.SetOverrideTarget(Target.position);
            input.SetOwnerSpawnOffset(new(0f, 0.5f));
            Projectile.SpawnProjectileGraph(graph, input, OnProjectileSpawn);
        }
        private void OnProjectileSpawn(Projectile p, Transform owner, Transform target)
        {
            p.SetFaction(faction);
        }
    }
}
