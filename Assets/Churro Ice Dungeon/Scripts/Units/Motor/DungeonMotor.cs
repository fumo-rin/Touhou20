using UnityEngine;

namespace ChurroIceDungeon
{

    public abstract class DungeonMotor : ScriptableObject
    {
        public struct MotorOutput
        {
            public float NextMoveTime;
            public bool Failed;
        }
        public abstract void PerformMotor(DungeonUnit unit, Vector2 input, out MotorOutput result);
    }
}
