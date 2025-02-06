using Mono.CSharp;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BremseTouhou
{
    [CreateAssetMenu(menuName = "Bremse Touhou/Attack Timeline")]
    public class UnitAttackTimeline : UnitAttack, IRetargetAttack
    {
        public List<TimelineEntry> attackTimeline = new();
        public override void AttackTarget(BaseUnit owner, Vector2 origin, Vector2 target, float addedAngle)
        {
            if (!owner.Alive)
            {
                return;
            }
            PlaySound(owner);
            AttackTimelineHandler.StartTimeline(this, owner, origin, target, addedAngle);
        }
        public void AttackWithRetargetting(BaseUnit owner, BaseUnit target, Vector2 origin, float addedAngle)
        {
            if (!owner.Alive)
            {
                return;
            }
            PlaySound(owner);
            AttackTimelineHandler.StartTimeline(this, owner, origin, target.Center, addedAngle, target);
        }
    }
    [System.Serializable]
    public class TimelineEntry
    {
        public UnitAttack Attack;
        public bool FollowOwner;
        public float AddedDelayInSeconds = 0f;
        public float AddedAngle = 0f;
        public bool Retargetting;
    }
}
