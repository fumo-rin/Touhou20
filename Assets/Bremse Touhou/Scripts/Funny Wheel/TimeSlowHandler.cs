using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Dan.Models;
namespace BremseTouhou
{
    public class TimeSlowHandler : MonoBehaviour
    {
        static TimeSlowHandler Instance;
        public class SlowdownEntry
        {
            public SlowdownEntry(float amount, float duration)
            {
                this.slowdown = amount;
                this.remainingDuration = duration;
            }
            public float slowdown;
            public float remainingDuration;
        }
        bool isPaused;
        static List<SlowdownEntry> timeSlow = new();
        private void Update()
        {
            SlowdownEntry item;
            for (var i = 0; i < timeSlow.Count; i++)
            {
                item = timeSlow[i];
                item.remainingDuration -= Time.unscaledDeltaTime;
                if (item.remainingDuration <= 0f)
                {
                    timeSlow.RemoveAt(i);
                    i--;
                }
            }
            Time.timeScale = GetTimescale();
        }
        public static void AddSlow(float amount, float length)
        {
            if (Instance == null || Instance.gameObject == null)
            {
                GameObject g = new GameObject("Gameobject");
                Instance = g.AddComponent<TimeSlowHandler>();
            }
            timeSlow.Add(new SlowdownEntry(amount, length));
        }
        private void OnDestroy()
        {
            if (Instance == this)
            {
                Instance = null;
            }
        }
        private float GetTimescale()
        {
            if (isPaused)
                return 0f;
            float highestSpeedup = 1f;
            float lowestTimeSlow = 1f;
            foreach (var entry in timeSlow)
            {
                if (entry.slowdown < lowestTimeSlow)
                {
                    lowestTimeSlow = entry.slowdown;
                }
                if (entry.slowdown > highestSpeedup)
                {
                    highestSpeedup = entry.slowdown;
                }
            }
            return lowestTimeSlow * highestSpeedup;
        }
    }
}
