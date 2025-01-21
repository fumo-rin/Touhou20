using UnityEngine;
using UnityEngine.Timeline;

namespace Bremsengine
{
    [TrackBindingType(typeof(BossDashAction))]
    [TrackClipType(typeof(BossDashClip))]
    public class BossDashTimeline : TrackAsset
    {

    }
}
