using UnityEngine;
using UnityEngine.Playables;

namespace Bremsengine
{
    public class BossDashBehaviour : PlayableBehaviour
    {
        public Dash activeDash;
        public override void ProcessFrame(Playable playable, FrameData info, object playerData)
        {
            BossDashAction handler = playerData as BossDashAction;
            if (handler == null)
            {
                Debug.LogError("Bad");
                return;
            }
            if (!Application.isPlaying)
            {
                return;
            }
            handler.TryDash(activeDash);
        }
    }
}
