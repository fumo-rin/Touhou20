using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BremseTouhou
{
    [CreateAssetMenu(fileName = "Touhou Motor", menuName = "Bremse Touhou/Touhou Motor")]
    public class TouhouMotor : UnitMotor
    {
        public override void Move(Rigidbody2D rb, Vector2 input, ref float nextMoveTime)
        {
            Debug.Log(rb);
            if (Time.time < nextMoveTime)
            {
                rb.velocity = Vector2.zero;
                return;
            }
            nextMoveTime = Time.time;
            rb.velocity = input * maxSpeed;
        }
    }
}
