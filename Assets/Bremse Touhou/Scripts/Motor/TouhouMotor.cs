using Core.Extensions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BremseTouhou
{
    [CreateAssetMenu(fileName = "Touhou Motor", menuName = "Bremse Touhou/Motor/Touhou Player")]
    public class TouhouMotor : UnitMotor
    {
        public override void Move(Rigidbody2D rb, Vector2 input, ref float nextMoveTime)
        {
            if (Time.time < nextMoveTime)
            {
                rb.linearVelocity = Vector2.zero;
                return;
            }
            nextMoveTime = Time.time;
            rb.linearVelocity = input * maxSpeed;
        }
    }
}
