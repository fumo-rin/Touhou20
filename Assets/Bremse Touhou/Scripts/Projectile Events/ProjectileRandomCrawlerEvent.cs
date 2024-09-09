using Bremsengine;
using Core.Extensions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BremseTouhou
{
    [CreateAssetMenu(menuName = "Bremse Touhou/Projectile Event/Random Crawler")]
    public class ProjectileRandomCrawlerEvent : ProjectileEvent
    {
        [SerializeField] float damage = 10f;
        [SerializeField] UnitAttack attack;
        [SerializeField] bool destroyOnEvent;
        public override void PerformEvent(Projectile p, Vector2 target)
        {
            attack.AttackTarget(null, p.transform.position, target, 0f);
            if (destroyOnEvent)
            {
                Destroy(p.gameObject);
            }
        }
    }
}
