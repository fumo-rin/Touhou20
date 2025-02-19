using UnityEngine;

namespace ChurroIceDungeon
{
    public interface IDamageable
    {
        public float CurrentHealth { get; set; }
        public void Hurt(float damage, Vector2 damagePosition);
    }
}
