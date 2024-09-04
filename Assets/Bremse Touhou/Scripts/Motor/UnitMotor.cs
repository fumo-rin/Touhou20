using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BremseTouhou
{
    public abstract class UnitMotor : ScriptableObject
    {
        [SerializeField] protected float maxSpeed = 4f;
        public abstract void Move(Rigidbody2D rb, Vector2 input, ref float nextMoveTime);
    }
}
