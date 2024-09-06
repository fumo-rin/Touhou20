using Mono.CSharp;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BremseTouhou
{
    [CreateAssetMenu(menuName = "Bremse Touhou/Attack Timeline")]
    public class UnitAttackTimeline : UnitAttack
    {
        public List<TimelineEntry> attackTimeline = new();
        public override void AttackTarget(BaseUnit owner, Vector2 origin, Vector2 target)
        {
            AttackTimelineHandler.StartTimeline(this, owner, origin, target);
        }
    }
    [System.Serializable]
    public class TimelineEntry
    {
        public UnitAttack Attack;
        public float AddedDelayInSeconds = 0f;
    }
}
