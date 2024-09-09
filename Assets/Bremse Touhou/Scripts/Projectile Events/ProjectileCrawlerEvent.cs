using Bremsengine;
using Core.Extensions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BremseTouhou
{
    [CreateAssetMenu(menuName = "Bremse Touhou/Projectile Event/Crawler")]
    public class ProjectileCrawlerEvent : ProjectileEvent
    {
        [SerializeField] UnitAttack attack;
        [SerializeField] bool destroyOnEvent;
        [SerializeField] bool isTargeted;
        public override void PerformEvent(Projectile p, Vector2 target)
        {
            if (!isTargeted)
            {
                target = (Vector2)p.transform.position + Random.insideUnitCircle.Shift(5f);
            }
            attack.AttackTarget(null, p.transform.position, target, 0f);
            if (destroyOnEvent)
            {
                Destroy(p.gameObject);
            }
        }
    }
}
