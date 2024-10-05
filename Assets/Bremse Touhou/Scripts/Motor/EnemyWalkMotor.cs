using Core.Extensions;
using UnityEngine;

namespace BremseTouhou
{
    [CreateAssetMenu(fileName = "Touhou Motor", menuName = "Bremse Touhou/Motor/Enemy Walk")]
    public class EnemyWalkMotor : UnitMotor
    {
        [SerializeField] float friction = 6f;
        [SerializeField] float acceleration = 2.5f;
        [SerializeField] float minimumSpeed = 0.4f;
        public override void Move(Rigidbody2D rb, Vector2 input, ref float nextMoveTime)
        {
            if (Time.time < nextMoveTime)
            {
                return;
            }
            nextMoveTime = Time.time;
            if (input == Vector2.zero)
            {
                rb.VelocityTowards(Vector2.zero, friction);
            }
            rb.VelocityTowards(input.normalized * maxSpeed, acceleration);
            if (rb.linearVelocity != Vector2.zero && rb.linearVelocity.magnitude < minimumSpeed)
            {
                rb.linearVelocity = input.ScaleToMagnitude(minimumSpeed);
            }
        }
    }
}
