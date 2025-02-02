using UnityEngine;

namespace BremseTouhou
{
    public class TurboOutcome : WheelOutcome
    {
        public override void ApplyEffect(BaseUnit unit)
        {
            TimeSlowHandler.AddSlow(1.25f, GetDuration());
        }

        public override void GameReset(BaseUnit unit)
        {

        }

        public override float GetDuration()
        {
            return 10f;
        }

        public override void RemoveEffect(BaseUnit unit)
        {

        }
    }
}
