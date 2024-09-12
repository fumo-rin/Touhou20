using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BremseTouhou
{
    public class TestAttackTimer : MonoBehaviour
    {
        public BaseUnit trackedTarget;
        public float timeDelay = 0.65f;
        float nextAttack;
        public UnitAttack attack;
        [SerializeField] BaseUnit owner;
        private void Update()
        {
            if (Time.time > nextAttack)
            {
                nextAttack = Time.time + timeDelay;
                if (attack is IRetargetAttack r)
                {
                    r.AttackWithRetargetting(owner == null ? null : owner, trackedTarget, transform.position, 0f);
                    return;
                }
                Attack((Vector2)transform.position + (trackedTarget.Center - (Vector2)transform.position));
            }
        }
        public void Attack(Vector2 target)
        {
            attack.AttackTarget(owner == null ? null : owner, transform.position, target, 0f);
        }
    }
}
