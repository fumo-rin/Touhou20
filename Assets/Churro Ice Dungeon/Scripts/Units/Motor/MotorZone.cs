using UnityEngine;

namespace ChurroIceDungeon
{
    public class MotorZone : MonoBehaviour
    {
        [SerializeField] float FrictionModifier = 1f, SpeedModifier = 1f, AccelerationModifier = 1f;
        public float Friction => FrictionModifier;
        public float Speed => SpeedModifier;
        public float Acceleration => AccelerationModifier;
        public float AccelerationOverride = 3f;
        public float FrictionOverride = 3f;
        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.GetComponent<DungeonUnit>() is DungeonUnit unit and not null)
            {
                unit.BindMotorZone(this);
            }
        }
        private void OnTriggerExit2D(Collider2D collision)
        {
            if (collision.GetComponent<DungeonUnit>() is DungeonUnit unit and not null)
            {
                unit.ReleaseMotorZone(this);
            }
        }
    }
}
