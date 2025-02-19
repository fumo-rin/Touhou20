using UnityEngine;
using System.Collections.Generic;
using Core.Extensions;

namespace ChurroIceDungeon
{
    [CreateAssetMenu(fileName ="Strafeprofile",menuName ="Churro/Strafe Profile")]
    public class StrafeProfile : ScriptableObject
    {
        [System.Serializable]
        public class Entry
        {
            public Vector2 distanceRange = new(0f, 5f);
            public float strafeAngle = 0f;
            public Vector2 strafeFlipTimeRange = new(1.5f, 2.5f);
            public bool canFlip;
        }
        [SerializeField] List<Entry> ranges = new();
        public bool TrySolveDistance(float distance, out Entry output)
        {
            output = null;
            foreach (Entry e in ranges)
            {
                if (distance.IsBetween(e.distanceRange.x, e.distanceRange.y))
                {
                    output = e;
                    return output != null;
                }
            }
            return output != null;
        }
    }
}
