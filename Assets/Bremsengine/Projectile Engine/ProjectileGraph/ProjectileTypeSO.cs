using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Bremsengine
{
    [CreateAssetMenu(menuName = "Bremsengine/Projectile2/Projectile Type")]
    public class ProjectileTypeSO : ScriptableObject
    {
        public Projectile Prefab;
    }
}
