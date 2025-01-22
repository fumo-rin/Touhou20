using Core.Extensions;
using System.Collections;
using UnityEngine;

namespace Bremsengine
{
    [CreateAssetMenu(menuName = "Bremsengine/Projectile Mods/Accelerate", fileName = "Accelerate")]
    public class ProjectileModAccelerate : ProjectileMod
    {
        [SerializeField] float maxSpeed = 10f;
        [SerializeField] float acceleration = 3f;
        protected override void AddModTo(Projectile p)
        {
            p.AddMod(this);
        }
        public override void RunMod(Projectile p, float remainingDuration, out float newDuration)
        {
            newDuration = remainingDuration - Time.deltaTime;
            if (duration < 0f)
            {
                return;
            }
            p.Post_SetNewVelocity(p.Velocity.LerpTowards(p.Velocity.ScaleToMagnitude(maxSpeed), acceleration));
            p.ApplyCurrentVelocity();
        }
        protected override IEnumerator CO_ModifierPayload(Projectile p, WaitForSeconds delay)
        {
            yield return delay;
            AddModTo(p);
            /*
            float iterationTime = duration;
            while (iterationTime > 0)
            {
                p.Post_SetNewVelocity(p.Velocity.LerpTowards(p.Velocity.ScaleToMagnitude(maxSpeed), acceleration));
                p.ApplyCurrentVelocity();
                iterationTime -= Time.deltaTime;
                yield return GetTickratedDelay();
            }*/
        }
    }
}
