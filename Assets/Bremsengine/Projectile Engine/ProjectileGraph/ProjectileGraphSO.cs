using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Bremsengine
{
    [CreateAssetMenu(menuName ="Bremsengine/Projectile2/Graph")]
    public class ProjectileGraphSO : ScriptableObject
    {
        public List<ProjectileNodeSO> nodes = new();
    }
}