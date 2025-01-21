using UnityEngine;
using UnityEngine.Playables;

namespace Bremsengine
{
    public class AttackTimelineBehaviour : PlayableBehaviour
    {
        public ProjectileGraphSO attack;
        public override void ProcessFrame(Playable playable, FrameData info, object playerData)
        {
            AttackHandler handler = playerData as AttackHandler;
            if (handler == null)
            {
                Debug.LogError("Bad");
                return;
            }
            if (!Application.isPlaying)
            {
                return;
            }
            if (attack != null && handler.ContainedAttack is ProjectileAttack p)
            {
                p.SetAttackGraph(attack);
            }
            handler.TriggerAttack(handler.ContainedAttack);
        }
    }
}
