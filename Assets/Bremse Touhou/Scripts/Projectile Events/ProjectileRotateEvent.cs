using Bremsengine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BremseTouhou
{
    [CreateAssetMenu(menuName = "Bremse Touhou/Projectile Event/Rotate")]
    public class ProjectileRotateEvent : ProjectileEvent
    {
        [SerializeField] float rotationPerSecond;
        public override void PerformEvent(Projectile p, BaseUnit owner, Vector2 target)
        {
            p.StartCoroutine(CO_AddRotationEveryFrame(p));

        }
        WaitForFixedUpdate wait = new WaitForFixedUpdate();
        private IEnumerator CO_AddRotationEveryFrame(Projectile p)
        {
            while (true)
            {
                p.Post_AddRotation(rotationPerSecond * Time.fixedDeltaTime);
                yield return wait;
            }
        }
    }
}
