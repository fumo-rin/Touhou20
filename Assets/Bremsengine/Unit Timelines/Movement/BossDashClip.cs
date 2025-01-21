using UnityEngine;
using UnityEngine.Playables;

namespace Bremsengine
{
    public class BossDashClip : PlayableAsset
    {
        [SerializeField] Dash containedDash;
        public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
        {
            var playable = ScriptPlayable<BossDashBehaviour>.Create(graph);

            BossDashBehaviour dashHandler = playable.GetBehaviour();
            dashHandler.activeDash = (Dash)containedDash.Clone();

            return playable;
        }
    }
}
