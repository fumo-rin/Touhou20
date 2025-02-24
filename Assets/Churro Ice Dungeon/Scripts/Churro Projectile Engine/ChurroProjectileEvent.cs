using Core.Extensions;
using UnityEngine;

namespace ChurroIceDungeon
{
    public abstract class ChurroProjectileEvent
    {
        bool wasActive;
        float tickStartTime;
        float tickEndTime;
        public void ApplySettings(eventSettings settings)
        {
            wasActive = false;
            tickStartTime = Time.time + settings.delay;
            tickEndTime = Time.time + settings.delay + settings.duration;
        }
        public struct eventSettings
        {
            public float delay;
            public float duration;
            public eventSettings(float duration, float delay)
            {
                this.delay = delay;
                this.duration = duration;
            }
        }
        public void TickEvent(ChurroProjectile eventProjectile, float deltaTime)
        {
            if (Time.time.IsBetween(tickStartTime, tickEndTime))
            {
                if (!wasActive)
                {
                    OnFirstRunPayload(eventProjectile);
                    wasActive = true;
                }
                RunPayload(eventProjectile, deltaTime);
            }
        }
        protected abstract void OnFirstRunPayload(ChurroProjectile eventProjectile);
        protected abstract void RunPayload(ChurroProjectile eventProjectile, float deltaTime);
    }
}