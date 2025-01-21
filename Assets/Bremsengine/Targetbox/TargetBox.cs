using UnityEngine;
using UnityEngine.Events;

namespace Bremsengine
{
    public class TargetBox : MonoBehaviour, IFaction, IProjectileHitListener
    {
        public delegate void DamageEvent(float damage, Vector2 position);
        public DamageEvent OnTakeDamage;
        [SerializeField] float hitboxDamageMultiplier = 1f;
        public IFaction FactionInterface => ((IFaction)this);
        BremseFaction IFaction.Faction { get; set; }
        [SerializeField] BremseFaction Faction;
        public bool SendDamageEvent(float damage, Vector2 damagePosition)
        {
            if (damage > 0f)
            {
                OnTakeDamage?.Invoke(damage * hitboxDamageMultiplier, damagePosition);
                return true;
            }
            return false;
        }
        private void Awake()
        {
            FactionInterface.SetFaction(Faction);
        }
        bool IProjectileHitListener.OnProjectileHit(Projectile p)
        {
            if (!FactionInterface.IsFriendsWith(p.Faction))
            {
                return SendDamageEvent(p.Damage, p.Position);
            }
            return false;
        }
    }
}
