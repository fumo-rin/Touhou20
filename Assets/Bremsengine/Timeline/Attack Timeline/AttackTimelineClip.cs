using UnityEngine;
using UnityEngine.Playables;

namespace Bremsengine
{
    public class AttackTimelineClip : PlayableAsset
    {
        public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
        {
            var playable = ScriptPlayable<AttackTimelineBehaviour>.Create(graph);

            AttackTimelineBehaviour attack = playable.GetBehaviour();

            return playable;
        }
    }
}
