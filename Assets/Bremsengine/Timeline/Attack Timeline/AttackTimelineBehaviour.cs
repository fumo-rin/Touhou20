using UnityEngine;
using UnityEngine.Playables;

namespace Bremsengine
{
    public class AttackTimelineBehaviour : PlayableBehaviour
    {
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
            handler.TriggerAttack(handler.ContainedAttack);
        }
    }
}
