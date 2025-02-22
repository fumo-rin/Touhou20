using UnityEngine;

namespace ChurroIceDungeon
{
    public class OrinFairy : ChurroBaseAttack
    {
        [SerializeField] ChurroProjectile projectile;
        [SerializeField] float attackSpeed = 0.6f;
        protected override void AttackPayload(ChurroProjectile.InputSettings input)
        {
            ChurroProjectile.SingleSettings s = new(0f, Hardmode ? 6f : 2.5f);
            float attackSpeed = this.attackSpeed;
            if (Hardmode)
            {
                attackSpeed = this.attackSpeed * 0.5f;
            }
            handler.settings.SetSwingDuration(attackSpeed);
            handler.settings.SetStallDuration(attackSpeed * 0.3f);
            ChurroProjectile.SpawnSingle(projectile, input, s);
        }
    }
}
