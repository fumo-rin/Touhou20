using Core.Extensions;
using UnityEngine;

namespace ChurroIceDungeon
{
    [DefaultExecutionOrder(-1)]
    public class AttackHandler : MonoBehaviour
    {
        #region Attack Time Settigns
        [System.Serializable]
        public class AttackTimeSettings
        {
            [SerializeField] bool SpreadVariation;
            public float NextAttackTime;
            [field: SerializeField] public float SwingDuration { get; private set; } = 0.2f;
            [field: SerializeField] public float StallDuration { get; private set; } = 0f;
            public AttackTimeSettings SetSwingDuration(float swingDuration)
            {
                this.SwingDuration = swingDuration;
                return this;
            }
            public AttackTimeSettings SetStallDuration(float swingDuration)
            {
                this.StallDuration = swingDuration;
                return this;
            }
            public void TriggerAttackTime() => NextAttackTime = Time.time + SwingDuration * 1f.Spread(SpreadVariation ? 15f : 0f);
            public void SetNewAttackTime(float time) => NextAttackTime = time + Time.time;
            public bool IsAttackTimeAllowed() => Time.time >= NextAttackTime;
            public float NewStallTime => Time.time + StallDuration * 1f.Spread(SpreadVariation ? 15f : 0f);
            public AttackTimeSettings SetSpreadVariation(bool state)
            {
                SpreadVariation = state;
                return this;
            }
            public AttackTimeSettings ApplySettings(AttackTimeSettings other)
            {
                this.SetSwingDuration(other.SwingDuration);
                this.SetStallDuration(other.StallDuration);
                this.SetSpreadVariation(other.SpreadVariation);
                return this;
            }
        }
        #endregion
        public DungeonUnit Owner { get; private set; }
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
        protected virtual void WhenStart()
        {

        }
        protected virtual void WhenDestroy()
        {

        }
        public void SetOwner(DungeonUnit owner)
        {
            this.Owner = owner;
        }
        private void Start()
        {
            WhenStart();
        }
        private void OnDestroy()
        {
            WhenDestroy();
        }
        public bool TryAttack(Vector2 target)
        {
            if (!settings.IsAttackTimeAllowed())
            {
                return false;
            }
            OnAttack?.Invoke(target);
            return true;
        }
    }
}