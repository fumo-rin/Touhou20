using Bremsengine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BremseTouhou
{
    [CreateAssetMenu(menuName = "Bremse Touhou/Projectile Event/Accelerate")]
    public class ProjectileAccelerateEvent : ProjectileEvent
    {
        [SerializeField] float acceleration = 0.5f;
        public override void PerformEvent(Projectile p, BaseUnit owner, Vector2 target)
        {
            p.StartCoroutine(CO_AddRotationEveryFrame(p));
        }
        WaitForFixedUpdate wait = new WaitForFixedUpdate();
        private IEnumerator CO_AddRotationEveryFrame(Projectile p)
        {
            while (true)
            {
                p.Post_AddVelocity(acceleration * Time.fixedDeltaTime);
                yield return wait;
            }
        }
    }
}
