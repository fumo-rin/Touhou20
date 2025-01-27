using UnityEngine;
using UnityEngine.Playables;
namespace Bremsengine
{
    [DefaultExecutionOrder(10)]
    public class SplinePathClip : PlayableAsset
    {
        public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
        {
            var playable = ScriptPlayable<SplinePathClipRunner>.Create(graph);

            SplinePathClipRunner handler = playable.GetBehaviour();

            return playable;
        }
    }
}
