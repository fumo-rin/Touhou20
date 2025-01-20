using Core.Extensions;
using System.Collections;
using UnityEngine;

namespace Bremsengine
{
    [CreateAssetMenu(menuName ="Bremsengine/Projectile Mods/Accelerate",fileName ="Accelerate")]
    public class ProjectileModAccelerate : ProjectileMod
    {
        [SerializeField] float duration = 0.5f;
        [SerializeField] float maxSpeed = 10f;
        [SerializeField] float acceleration = 3f;
        protected override IEnumerator CO_ModifierPayload(Projectile p, WaitForSeconds delay)
        {
            yield return delay;
            float iterationTime = duration;
            while (iterationTime > 0)
            {
                p.Post_SetNewVelocity(p.Velocity.LerpTowards(p.Velocity.ScaleToMagnitude(maxSpeed), acceleration));
                p.ApplyCurrentVelocity();
                iterationTime -= Time.deltaTime;
                yield return null;
            }
        }
    }
}
