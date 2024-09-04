using Core.Extensions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Bremsengine
{
    [CreateAssetMenu(fileName = "New Projectile", menuName = "Bremsengine/Projectile")]
    public class ProjectileSO : ScriptableObject
    {
        public Projectile projectilePrefab;
        [SerializeField] Vector2 angleSpread = new(-2.5f, 2.5f);
        [field: SerializeField] public Vector2 SpeedRange { get; protected set; } = new(5f, 5.5f);
        public float Spread => angleSpread.RandomBetweenXY();
    }
}
