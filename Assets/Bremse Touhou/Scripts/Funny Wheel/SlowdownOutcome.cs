using UnityEngine;
using Bremsengine;

namespace BremseTouhou
{
    public class SlowdownOutcome : WheelOutcome
    {
        public override void ApplyEffect(BaseUnit unit)
        {
            TimeSlowHandler.AddSlow(0.5f, GetDuration());
        }

        public override void GameReset(BaseUnit unit)
        {
            
        }

        public override float GetDuration()
        {
            return 15f;
        }

        public override void RemoveEffect(BaseUnit unit)
        {

        }
    }
}