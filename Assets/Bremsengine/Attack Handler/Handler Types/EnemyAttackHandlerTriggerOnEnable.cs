using UnityEngine;

namespace Bremsengine
{
    public class EnemyAttackHandlerTriggerOnEnable : MonoBehaviour
    {
        [SerializeField] EnemyAttackHandler handler;
        private void OnEnable()
        {
            handler.ForceAttack(handler.ContainedAttack);
        }
    }
}
