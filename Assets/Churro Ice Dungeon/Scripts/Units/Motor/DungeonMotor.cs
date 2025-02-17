using UnityEngine;

namespace ChurroIceDungeon
{

    public abstract class DungeonMotor : ScriptableObject
    {
        [field: SerializeField] public float MaxSpeed { get; protected set; }
        public struct MotorOutput
        {
            public float NextMoveTime;
            public bool Failed;
        }
        public abstract void PerformMotor(DungeonUnit unit, Vector2 input, out MotorOutput result);
    }
}
