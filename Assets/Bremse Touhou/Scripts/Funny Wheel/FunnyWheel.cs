using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System.Data;
using Core.Extensions;
using System.Security.Cryptography;
using UnityEngine.Rendering;
using Bremsengine;
using TMPro;

namespace BremseTouhou
{
    public class FunnyWheel : MonoBehaviour
    {
        [System.Serializable]
        class WheelEffectsOverTime
        {
            public WheelEffectsOverTime(WheelOutcome o, float duration)
            {
                this.outcome = o;
                this.remainingDuration = duration;
            }
            public WheelOutcome outcome;
            public float remainingDuration;
        }
        [SerializeField] WheelSelector selector;
        [SerializeField] BaseUnit target;
        [SerializeField] List<WheelOutcome> outcomes = new();
        [SerializeField] Rigidbody2D rb;
        [SerializeField] TMP_Text tooltipText;
        WheelOutcome lastCollision;
        Coroutine co;
        float nextSpinTime;
        List<WheelEffectsOverTime> runner = new();
        private void SelectOutcome(int i)
        {
            SelectOutcome(outcomes[i]);
        }
        private void SelectOutcome(WheelOutcome w)
        {
            w.ApplyEffect(target);
            runner.Add(new WheelEffectsOverTime(w, w.GetDuration()));
            tooltipText.text = w.Tooltip;
            w.PlaySound();
        }
        public void GameReset()
        {
            foreach (var item in outcomes)
            {
                item.GameReset(target);
            }
            nextSpinTime = Time.time + 2f;
            tooltipText.text = "";
        }
        public void SetLastCollision(WheelOutcome c)
        {
            lastCollision = c;
        }
        public void ClearAllEffects()
        {
            runner.Clear();
        }
        private void Awake()
        {
            runner.Clear();
        }
        [QFSW.QC.Command("-spinwheel")]
        public void SpinWheel(int forcedSelection = -1)
        {
            if (forcedSelection >= 0)
            {
                SelectOutcome(forcedSelection);
            }
            else
            {
                if (co != null)
                {
                    StopCoroutine(co);
                }
                co = StartCoroutine(CO_Spin());
            }
        }
        private IEnumerator CO_Spin()
        {
            rb.angularVelocity = Random.Range(2500f, 3500f);
            yield return new WaitUntil(() => rb.angularVelocity <= 5f);
            rb.angularVelocity = 0f;
            WheelOutcome selection = lastCollision;
            if (selection != null)
            {
                selection.ApplyEffect(target);
                yield break;
            }
            if (selection == null)
            {
                float highestDot = -1f;
                Vector3 iteration;
                foreach (var item in outcomes)
                {
                    iteration = (item.transform.position - selector.transform.position).normalized;
                    float dot = Vector3.Dot(selector.transform.position + iteration, selector.transform.position + Vector3.up);
                    if (dot > highestDot)
                    {
                        highestDot = dot;
                        selection = item;
                    }
                }
                SelectOutcome(selection);
            }
            /*
            float shortestDistance = Mathf.Infinity;
            foreach (var item in outcomes)
            {
                if (selection == null)
                {
                    selection = item;
                    continue;
                }
                float distance = item.transform.position.DistanceTo(selection.transform.position);
                Debug.Log(item.transform.name+" : "+item.transform.position + " : " + selector.transform.position + " : " +distance.ToString("F1"));
                Debug.DrawLine(item.transform.position, selector.transform.position, Color.yellow, 1f);
                if (distance < shortestDistance)
                {
                    shortestDistance = distance;
                    selection = item;
                }
            }
            Debug.DrawLine(selection.transform.position, selector.transform.position, Color.green, 1f);
            Debug.Log("Selection : " + selection.transform.name);*/
        }
        private void Update()
        {
            if (nextSpinTime <= Time.time && !Dialogue.IsDialogueRunning)
            {
                nextSpinTime = Time.time + 10f;
                SpinWheel();
            }
            if (runner.Count <= 0)
                return;
            WheelEffectsOverTime item;
            for (int i = 0; i < runner.Count; i++)
            {
                item = runner[i];
                if (item.remainingDuration > 0f)
                {
                    item.remainingDuration -= Time.deltaTime;
                }
                if (item.remainingDuration <= 0f)
                {
                    runner.RemoveAt(i);
                    item.outcome.RemoveEffect(target);
                    i--;
                }
            }
        }
    }
}
