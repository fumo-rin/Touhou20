using Core.Extensions;
using UnityEngine;

namespace ChurroIceDungeon
{

    public abstract class DungeonMotor : ScriptableObject
    {
        [field: SerializeField] public float MaxSpeed { get; protected set; }
        [SerializeField] private float FailFriction = 6f;
        public bool HasMovedThisFrame { get; protected set; }
        public struct MotorOutput
        {
            public float NextMoveTime;
            public bool Failed;
        }
        public struct Settings
        {
            public DungeonUnit unit;
            public float SpeedMod;
            public float FrictionMod;
            public float AccelerationMod;
            public Settings(DungeonUnit unit)
            {
                this.unit = unit; SpeedMod = 1f; FrictionMod = 1f; AccelerationMod = 1f;
            }
        }
        public void ApplyFailFriction(DungeonUnit unit)
        {
            unit.RB.VelocityTowards(Vector2.zero, FailFriction);
        }
        public void RunMotor(DungeonUnit unit, Vector2 input, Settings settings, out MotorOutput result, ref float nextMoveTime)
        {
            PerformMotor(unit, input, settings, out result, ref nextMoveTime);
            if (result.Failed == false)
            {
                HasMovedThisFrame = false;
                return;
            }
            HasMovedThisFrame = true;
        }
        protected abstract void PerformMotor(DungeonUnit unit, Vector2 input, Settings settings, out MotorOutput result, ref float nextMoveTime);
    }
}
