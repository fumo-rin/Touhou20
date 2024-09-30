using UnityEngine;

namespace Bremsengine
{
    public abstract class AttackHandler : MonoBehaviour
    {
        [SerializeField] protected Transform owner;
        [SerializeField] BremseFaction faction;
        public Transform OwnerTransform => owner;
        public BremseFaction Faction => faction;
        public abstract bool CanAttack();
        public abstract void TriggerAttack(BaseAttack attack);
    }
}