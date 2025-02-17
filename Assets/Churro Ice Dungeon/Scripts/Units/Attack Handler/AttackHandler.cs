using UnityEngine;

namespace ChurroIceDungeon
{
    public class AttackHandler : MonoBehaviour
    {
        [System.Serializable]
        public struct AttackTimeSettings
        {
            public float NextAttackTime { get; private set; }
            public float SwingTime { get; private set; }
            public void SetSwingTime(float swingTime) => this.SwingTime = swingTime;
            public void TriggerAttackTime() => NextAttackTime = Time.time + SwingTime;
            public bool IsAttackTimeAllowed() => NextAttackTime <= Time.time;
        }
        public delegate void AttackAction(Vector2 target);
        public AttackAction OnAttack;
        public static Transform FallbackTarget;
        public static void SetFallbackTarget(Transform t) => FallbackTarget = t;
        public AttackTimeSettings settings { get; private set; }
        private void Start()
        {
            settings.SetSwingTime(0.2f);
        }
        public bool TryAttack(Vector2 target)
        {
            if (!settings.IsAttackTimeAllowed())
            {
                return false;
            }
            settings.TriggerAttackTime();
            OnAttack?.Invoke(target);
            return true;
        }
    }
}