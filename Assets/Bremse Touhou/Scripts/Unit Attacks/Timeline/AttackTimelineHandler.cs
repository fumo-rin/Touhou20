using Bremsengine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BremseTouhou
{
    public class AttackTimelineHandler : MonoBehaviour
    {
        static AttackTimelineHandler instance;
        public static void Validate()
        {
            if (instance == null)
            {
                instance = new GameObject().AddComponent<AttackTimelineHandler>();
                DontDestroyOnLoad(instance.gameObject);
            }
        }
        public static void StartTimeline(UnitAttackTimeline t, BaseUnit owner, Vector2 origin, Vector2 target, float addedAngle = 0f, BaseUnit targetInjection = null)
        {
            Validate();
            instance.StartCoroutine(instance.CO_TriggerTimeline(t, owner, origin, target, addedAngle, targetInjection));
        }
        private IEnumerator CO_TriggerTimeline(UnitAttackTimeline t, BaseUnit owner, Vector2 origin, Vector2 target, float addedAngle = 0f, BaseUnit targetInjection = null)
        {
            TimelineEntry entry;
            Vector2 attackPosition;
            Vector2 attackTarget;
            for (int i = 0; i < t.attackTimeline.Count; i++)
            {
                if (i >= t.attackTimeline.Count)
                {
                    continue;
                }
                entry = t.attackTimeline[i];

                if (entry.AddedDelayInSeconds > 0f)
                {
                    yield return new WaitForSeconds(entry.AddedDelayInSeconds);
                }

                attackPosition = entry.FollowOwner && owner ? owner.Center : origin;

                if (targetInjection != null && entry.Retargetting)
                {
                    attackTarget = targetInjection.Center;
                }
                else
                {
                    attackTarget = target;
                    if (entry.FollowOwner && owner)
                    {
                        Vector2 deltaPosition = owner.Center - origin;
                        attackTarget += deltaPosition;
                    }
                }
                if (entry.Attack == t)
                {
                    Debug.Log("Attack Timeline is triggering itself for : " + t.name);
                    continue;
                }
                entry.Attack.AttackTarget(owner, attackPosition, attackTarget, addedAngle + entry.AddedAngle);
            }
        }
    }
}
