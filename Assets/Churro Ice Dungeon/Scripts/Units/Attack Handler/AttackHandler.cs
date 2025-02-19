using Core.Extensions;
using UnityEngine;

namespace ChurroIceDungeon
{
    [DefaultExecutionOrder(-1)]
    public class AttackHandler : MonoBehaviour
    {
        [System.Serializable]
        public class AttackTimeSettings
        {
            [SerializeField] bool SpreadVariation;
            [field: SerializeField] public float NextAttackTime { get; private set; }
            [field: SerializeField] public float SwingDuration { get; private set; }
            [field: SerializeField] public float StallDuration { get; private set; }
            public void SetSwingDuration(float swingDuration) => this.SwingDuration = swingDuration;
            public void SetStallDuration(float swingDuration) => this.StallDuration = swingDuration;
            public void TriggerAttackTime() => NextAttackTime = Time.time + SwingDuration * 1f.Spread(SpreadVariation ? 15f : 0f);
            public bool IsAttackTimeAllowed() => Time.time >= NextAttackTime;
            public float NewStallTime => Time.time + StallDuration * 1f.Spread(SpreadVariation ? 15f : 0f);
        }
        public delegate void AttackAction(Vector2 target);
        public AttackAction OnAttack;
        public static Transform FallbackTarget;
        public static void SetFallbackTarget(Transform t) => FallbackTarget = t;
        [field: SerializeField] public AttackTimeSettings settings { get; private set; }
        private void Awake()
        {
            if (settings.SwingDuration <= 0f) settings.SetSwingDuration(0.65f);
            if (settings.StallDuration <= 0f) settings.SetStallDuration(0.35f);
        }
        public bool TryAttack(Vector2 target)
        {
            if (!settings.IsAttackTimeAllowed())
            {
                return false;
            }
            //Handle Sounds elsewhere pleae
            settings.TriggerAttackTime();
            OnAttack?.Invoke(target);
            return true;
        }
    }
}