using UnityEngine;

namespace Bremsengine
{
    public abstract class AttackHandler : MonoBehaviour
    {
        [SerializeField] protected Transform assignedTarget;
        protected float nextAttackTime;
        public BaseAttack ContainedAttack => CollapseBaseAttack();
        protected abstract BaseAttack CollapseBaseAttack();
        [SerializeField] protected Transform owner;
        [SerializeField] BremseFaction faction;
        public bool UnitAlive { get; protected set; }
        public void SetAlive(bool state)
        {
            UnitAlive = state;
        }
        protected abstract void WhenStart();
        private void Start()
        {
            WhenStart();
        }
        public Transform OwnerTransform => owner;
        public BremseFaction Faction => faction;
        public abstract bool CanAttack();
        public abstract void TriggerAttack(BaseAttack attack);
        public void ForceSetNewAttackDelay(float newDelay) => SetNextAttackDelay(newDelay);
        protected void SetNextAttackDelay(float delay) => nextAttackTime = Time.time + delay;
        public void AssignTarget(Transform t)
        {
            assignedTarget = t;
        }
        public abstract void ForceAttack(BaseAttack attack);
    }
}