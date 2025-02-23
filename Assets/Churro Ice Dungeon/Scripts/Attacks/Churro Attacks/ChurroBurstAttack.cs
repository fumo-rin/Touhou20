using Core.Extensions;
using System.Collections;
using UnityEngine;

namespace ChurroIceDungeon
{
    public class ChurroBurstAttack : ChurroBaseAttack
    {
        [SerializeField] ChurroProjectile projectilePrefab;
        [SerializeField] ChurroProjectile miniArc;
        [SerializeField] float burstDamage = 50f;
        [SerializeField] int burstAmount = 7;
        [SerializeField] float miniArcAmount = 7f;
        [SerializeField] float burstDuration = 0.35f;
        [SerializeField] float spread;
        float TimePerShot => burstDuration / burstAmount;
        float DamagePerBurst => burstDamage / burstAmount;
        protected override void OnAttackLoad()
        {
            base.OnAttackLoad();
        }
        protected override void AttackPayload(ChurroProjectile.InputSettings input)
        {
            StartCoroutine(CO_Burst());
            foreach (var item in Arc(-15f, 15, 49.9f / miniArcAmount, 13f).Spawn(input, miniArc))
            {
                item.Action_AddRotation(1.5f.Spread(100f)).Action_SetDamage(1f).Action_MultiplyVelocity(1f.Spread(7f));
            }
            IEnumerator CO_Burst()
            {
                ChurroProjectile iteration = null;
                WaitForSeconds wait = new WaitForSeconds(TimePerShot);
                ChurroProjectile.SingleSettings single = new(0f, 27f);
                Vector2 pos = owner.CurrentPosition;
                for (int i = 0; i < burstAmount; i++)
                {
                    pos = owner.CurrentPosition;
                    input.SetOrigin(pos);
                    iteration = ChurroProjectile.SpawnSingle(projectilePrefab, input, single);
                    iteration.Action_SetDamage(DamagePerBurst).Action_MultiplyVelocity(1f.Spread(7f)).Action_AddRotation(spread.Spread(100f));
                    yield return wait;
                }
            }
        }
    }
}