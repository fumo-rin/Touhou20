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
            float lastTime = Time.time;
            float deltaTime;
            while (iterationTime > 0)
            {
                deltaTime = Time.time - lastTime;
                iterationTime += deltaTime;
                p.Post_SetNewVelocity(p.Velocity.LerpTowardsWithDeltaTime(p.Velocity.ScaleToMagnitude(maxSpeed), acceleration, deltaTime));
                p.ApplyCurrentVelocity();
                iterationTime -= deltaTime;
                yield return GetTickratedDelay();
            }
        }
    }
}
