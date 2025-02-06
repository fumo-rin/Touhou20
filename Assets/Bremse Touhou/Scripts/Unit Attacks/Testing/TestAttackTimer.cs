using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BremseTouhou
{
    public class TestAttackTimer : MonoBehaviour
    {
        public float timeDelay = 0.65f;
        float nextAttack;
        public UnitAttack attack;
        [SerializeField] BaseUnit owner;
        private void Update()
        {
            if (Time.time > nextAttack)
            {
                nextAttack = Time.time + timeDelay;
                if (owner == null)
                {
                    AttackPosition((Vector2)transform.position + Vector2.down);
                    return;
                }
                if (!owner.HasTarget)
                {
                    AttackPosition(owner.Center + Vector2.down);
                    return;
                }
                if (!owner.Alive)
                    return;
                if (attack is IRetargetAttack r)
                {
                    r.AttackWithRetargetting(owner, owner.Target, transform.position, 0f);
                    return;
                }
                AttackPosition((Vector2)transform.position + (owner.Target.Center - (Vector2)transform.position));
            }
        }
        private void AttackPosition(Vector2 target)
        {
            attack.AttackTarget(owner == null ? null : owner, transform.position, target, 0f);
        }
    }
}
