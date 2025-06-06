using System;
using UnityEngine;
using UnityEngine.UI;

namespace Core.Extensions
{
    public static class UnityUIExtensions
    {
        public static void ClearAndBind(this Button b, Action call)
        {
            b.onClick.RemoveAllListeners();
            b.onClick.AddListener(() => call?.Invoke());
        }
        public static void AddClickAction(this Button b, Action c)
        {
            b.onClick.AddListener(() => c?.Invoke());
        }
        public static void RemoveClickAction(this Button b, Action c)
        {
            b.onClick.RemoveListener(() => c?.Invoke());
        }
    }
}
