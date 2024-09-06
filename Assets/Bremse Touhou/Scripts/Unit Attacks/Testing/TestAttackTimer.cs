using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BremseTouhou
{
    public class TestAttackTimer : MonoBehaviour
    {
        public Transform trackedTarget;
        public float timeDelay = 0.65f;
        float nextAttack;
        public UnitAttack attack;
        private void Update()
        {
            if (Time.time > nextAttack)
            {
                nextAttack = Time.time + timeDelay;
                Attack((Vector2)transform.position + (Vector2)(trackedTarget.position - transform.position));
            }
        }
        public void Attack(Vector2 target)
        {
            attack.AttackTarget(null, transform.position, target);
        }
    }
}
