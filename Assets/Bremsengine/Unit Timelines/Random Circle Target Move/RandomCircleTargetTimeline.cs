using UnityEngine;
using UnityEngine.Timeline;

namespace Bremsengine
{
    [TrackBindingType(typeof(Transform))]
    [TrackClipType(typeof(RandomCircleTargetClip))]
    public class RandomCircleTargetTimeline : PlayableTrack
    {

    }
}
