using Core.Extensions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Bovine
{
    [CreateAssetMenu(fileName ="New Projectile", menuName ="Bovine/Projectile")]
    public class ProjectileSO : ScriptableObject
    {
        public Projectile projectilePrefab;
        [SerializeField] Vector2 angleSpread = new(-2.5f, 2.5f);
        public float Spread => angleSpread.RandomBetweenXY();
    }
}
