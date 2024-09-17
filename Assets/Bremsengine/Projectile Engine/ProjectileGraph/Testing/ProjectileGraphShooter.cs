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
            Projectile.SpawnProjectileGraph(graph, owner, testTarget, OverridePosition, OnProjectileSpawn);
        }
        private void OnProjectileSpawn(Projectile p, Transform owner, Transform target)
        {
            p.SetFaction(faction);
        }
    }
}
