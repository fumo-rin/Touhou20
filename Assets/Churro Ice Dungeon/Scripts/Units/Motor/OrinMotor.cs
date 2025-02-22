using Core.Extensions;
using UnityEngine;

namespace ChurroIceDungeon
{
    [CreateAssetMenu(fileName = "New Motor", menuName = "Churro/Motor/Orin")]
    public class OrinMotor : DungeonMotor
    {
        [SerializeField] float cooldown = 0.6f;
        [SerializeField] float speedSpread = 15f;
        protected override void PerformMotor(DungeonUnit unit, Vector2 input, Settings settings, out MotorOutput result, ref float nextMoveTime)
        {
            result = new MotorOutput();
            result.Failed = true;

            float finalSpeed = MaxSpeed * 1f.Spread(speedSpread);

            if (Time.time > nextMoveTime && input != Vector2.zero)
            {
                unit.RB.linearVelocity = input.normalized * finalSpeed;
                result.Failed = false;
                result.NextMoveTime = Time.time + (cooldown / settings.SpeedMod);
            }
        }
    }
}