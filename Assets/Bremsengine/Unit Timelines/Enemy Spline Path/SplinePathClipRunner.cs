using UnityEngine;
using UnityEngine.Playables;

namespace Bremsengine
{
    [DefaultExecutionOrder(10)]
    public class SplinePathClipRunner : PlayableBehaviour
    {
        public override void ProcessFrame(Playable playable, FrameData info, object playerData)
        {
            SplinePathMover handler = playerData as SplinePathMover;
            if (handler == null)
            {
                Debug.LogError("Bad");
                return;
            }
            if (!Application.isPlaying)
            {
                return;
            }
            handler.PerformCurrentPath();
        }
    }
}
