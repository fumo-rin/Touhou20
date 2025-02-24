using Core.Extensions;
using System;
using System.Collections;
using TMPro;
using UnityEngine;

namespace ChurroIceDungeon
{
    public class BossTimer : MonoBehaviour
    {
        static BossTimer instance;
        float timer;
        Coroutine coroutine;
        [SerializeField] TMP_Text text;
        private void Awake()
        {
            instance = this;
            coroutine = null;
            text.text = "";
        }
        IEnumerator CO_SetTimer(float value, Action nextAction)
        {
            timer = value;
            while (timer > 0)
            {
                timer -= Time.deltaTime;
                text.text = timer.Ceil().ToString("F0");
                yield return null;
            }
            yield return new WaitForSeconds(1f);
            nextAction?.Invoke();
            text.text = "";
            coroutine = null;
        }
        public static void SetTimer(float value, Action nextAction = null)
        {
            if (instance != null)
            {
                if (instance.coroutine != null)
                {
                    instance.StopCoroutine(instance.coroutine);
                    instance.coroutine = null;
                }
                instance.coroutine = instance.StartCoroutine(instance.CO_SetTimer(value, nextAction));
            }
        }
    }
}
