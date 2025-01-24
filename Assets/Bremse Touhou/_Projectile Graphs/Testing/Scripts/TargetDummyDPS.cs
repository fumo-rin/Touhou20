using Bremsengine;
using Core.Extensions;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BremseTouhou
{
    [System.Serializable]
    public class TargetDummyEntry
    {
        public float time;
        public float damage;
        public TargetDummyEntry(float damage)
        {
            this.time = Time.time;
            this.damage = damage;
        }
    }
    public class TargetDummyDPS : MonoBehaviour
    {
        [SerializeField] private int startingDummyEntries = 500;
        static int maxDummyEntries;
        [SerializeField] TargetBox dummyTargetBox;
        [SerializeField] TMP_Text dummyText;
        static Queue<TargetDummyEntry> dummyEntries;
        public static float totalDamage;
        public static float EndOfQueueTime;
        public static float GetDPS()
        {
            float dps = totalDamage / (Time.time - EndOfQueueTime).Max(0.05f);

            TargetDummyEntry entry = dummyEntries.Dequeue();
            Debug.Log(EndOfQueueTime);
            EndOfQueueTime = entry.time;
            totalDamage -= entry.damage;

            Debug.Log($"Damage : {totalDamage} : Timespan : {Time.time - EndOfQueueTime}");

            return dps;
        }
        private void Start()
        {
            dummyTargetBox.OnTakeDamage += AddDamageNumber;
            UpdateDPSText += SetDPSText;
            SetMaxDummyEntries(startingDummyEntries);
        }
        public void SetDummyEntries(int i)
        {
            SetMaxDummyEntries(i);
        }
        public void SetMaxDummyEntries(int value)
        {
            maxDummyEntries = value.Max(5);
            totalDamage = 0f;
            dummyEntries = new Queue<TargetDummyEntry>(value);
            dummyText.text = "Hit me! :3";
        }
        private void OnDestroy()
        {
            dummyTargetBox.OnTakeDamage -= AddDamageNumber;
            UpdateDPSText -= SetDPSText;
        }
        private void SetDPSText(float value)
        {
            if (value > 0f)
            {
                dummyText.text = value.ToString("F0");
            }
        }
        private void AddDamageNumber(float value, Vector2 position)
        {
            AddDummyEntry(new TargetDummyEntry(value));
        }
        public delegate void DummyTextEvent(float dps);
        public static DummyTextEvent UpdateDPSText;
        public static void AddDummyEntry(TargetDummyEntry e)
        {
            dummyEntries.Enqueue(e);
            totalDamage += e.damage;
            if (dummyEntries.Count >= maxDummyEntries)
            {
                UpdateDPSText?.Invoke(GetDPS());
            }
        }
    }
}
