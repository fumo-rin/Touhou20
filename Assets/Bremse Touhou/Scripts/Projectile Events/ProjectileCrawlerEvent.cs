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
        static Vector2 targetIteration;
        public override void PerformEvent(Projectile p, BaseUnit owner, Vector2 target)
        {
            targetIteration = target;
            if (!isTargeted)
            {
                targetIteration = (Vector2)p.transform.position + Random.insideUnitCircle.ScaleToMagnitude(5f);
            }
            attack.AttackTarget(owner, p.transform.position, targetIteration, 0f);
            if (destroyOnEvent)
            {
                Destroy(p.gameObject);
            }
        }
    }
}
