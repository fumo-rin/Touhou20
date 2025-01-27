using UnityEngine;
using UnityEngine.Timeline;

namespace Bremsengine
{
    [DefaultExecutionOrder(10)]
    [TrackBindingType(typeof(SplinePathMover))]
    [TrackClipType(typeof(SplinePathClip))]
    public class SplinePathTrack : PlayableTrack
    {

    }
}
