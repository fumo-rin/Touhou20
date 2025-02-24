using Core.Extensions;
using UnityEngine;

namespace ChurroIceDungeon
{
    public class ChurroEventRotate : ChurroProjectileEvent
    {
        public float CurrentRotationPerSecond = 0f;
        public float RotationTargetValue;
        public float Acceleration = 2f;

        public ChurroEventRotate(eventSettings settings, float rotationTarget, float acceleration)
        {
            ApplySettings(settings);
            RotationTargetValue = rotationTarget;
            this.Acceleration = acceleration;
        }

        protected override void OnFirstRunPayload(ChurroProjectile eventProjectile)
        {

        }

        protected override void RunPayload(ChurroProjectile eventProjectile, float deltaTime)
        {
            CurrentRotationPerSecond = CurrentRotationPerSecond.MoveTowards(RotationTargetValue, Acceleration);
            eventProjectile.Action_AddRotation(CurrentRotationPerSecond * deltaTime);
        }
    }
}
