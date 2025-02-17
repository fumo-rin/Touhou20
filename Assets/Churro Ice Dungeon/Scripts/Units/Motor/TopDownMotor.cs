using Core.Extensions;
using UnityEngine;

namespace ChurroIceDungeon
{
    [CreateAssetMenu(fileName = "New Motor", menuName = "Churro/Motor/Topdown")]
    public class TopDownMotor : DungeonMotor
    {
        [SerializeField] float Acceleration = 25f;
        [SerializeField] float MaxSpeed = 4f;
        [SerializeField] float Friction = 15f;
        public override void PerformMotor(DungeonUnit unit, Vector2 input, out MotorOutput result)
        {
            result = new MotorOutput();
            result.NextMoveTime = Time.time;
            result.Failed = false;

            float finalSpeed = MaxSpeed;

            if (input != Vector2.zero)
            {
                unit.RB.VelocityTowards(input.Clamp(0f, 1f) * finalSpeed, Acceleration);
            }
            else
            {
                unit.RB.VelocityTowards(Vector2.zero, Friction);
            }
        }
    }
}
