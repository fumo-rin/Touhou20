using Core.Extensions;
using UnityEngine;

namespace ChurroIceDungeon
{
    [CreateAssetMenu(fileName = "New Motor", menuName = "Churro/Motor/Topdown")]
    public class TopDownMotor : DungeonMotor
    {
        [Range(0f, 100f)]
        [SerializeField] float Acceleration = 25f;
        [Range(0f, 100f)]
        [SerializeField] float Friction = 15f;
        protected override void PerformMotor(DungeonUnit unit, Vector2 input, Settings settings, out MotorOutput result, ref float nextMoveTime)
        {
            result = new MotorOutput();
            result.NextMoveTime = Time.time;
            result.Failed = false;
            float Friction = this.Friction;
            float Acceleration = this.Acceleration;

            foreach (var item in unit.MotorZones)
            {
                if (item.FrictionOverride > 0f)
                {
                    Friction = item.FrictionOverride;
                }
                if (item.AccelerationOverride > 0f)
                {
                    Acceleration = item.AccelerationOverride;
                }
            }

            float finalSpeed = MaxSpeed * settings.SpeedMod;

            if (input != Vector2.zero)
            {
                if (Acceleration == 0f)
                {
                    unit.RB.linearVelocity = input.Clamp(0f, 1f) * finalSpeed;
                    return;
                }
                unit.RB.VelocityTowards(input.Clamp(0f, 1f) * finalSpeed, Acceleration * settings.AccelerationMod);
            }
            else
            {
                if (Friction == 0f)
                {
                    unit.RB.linearVelocity = Vector2.zero;
                    return;
                }
                unit.RB.VelocityTowards(Vector2.zero, Friction * settings.FrictionMod);
            }
        }
    }
}
