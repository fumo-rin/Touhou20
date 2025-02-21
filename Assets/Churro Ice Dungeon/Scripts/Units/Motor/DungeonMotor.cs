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
        public struct Settings
        {
            public float SpeedMod;
            public float FrictionMod;
            public float AccelerationMod;
            public Settings(float speedmod)
            {
                SpeedMod = speedmod; FrictionMod = 1f; AccelerationMod = 1f;
            }
        }
        public abstract void PerformMotor(DungeonUnit unit, Vector2 input, Settings settings, out MotorOutput result);
    }
}
