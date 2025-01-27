using UnityEngine;
using UnityEngine.Timeline;

namespace Bremsengine
{
    [TrackBindingType(typeof(AttackHandler))]
    [TrackClipType(typeof(AttackTimelineClip))]
    public class AttackTimelineTrack : TrackAsset
    {

    }
}