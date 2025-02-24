using Core.Extensions;
using UnityEngine;

namespace ChurroIceDungeon
{
    [System.Serializable]
    public class ChurroEventAccelerate : ChurroProjectileEvent
    {
        float acceleration;
        float targetSpeed;
        public ChurroEventAccelerate(eventSettings settings, float speed, float acceleration)
        {
            ApplySettings(settings);
            this.targetSpeed = speed;
            this.acceleration = acceleration;
        }
        protected override void OnFirstRunPayload(ChurroProjectile eventProjectile)
        {

        }
        protected override void RunPayload(ChurroProjectile eventProjectile, float deltaTime)
        {
            float currentSpeed = eventProjectile.CurrentVelocity.magnitude;
            currentSpeed = currentSpeed.MoveTowards(targetSpeed, deltaTime * acceleration);

            eventProjectile.Action_SetVelocity(eventProjectile.CurrentVelocity, currentSpeed);
        }
    }
}
