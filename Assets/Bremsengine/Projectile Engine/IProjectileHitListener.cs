using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Bremsengine
{
    public interface IProjectileHitListener : IFaction
    {
        /// <summary>
        /// Returns true if the projectile is supposed to have hit (example : same faction = no hit)
        /// </summary>
        /// <param name="p"></param>
        public bool OnProjectileHit(Projectile p);
    }
}
