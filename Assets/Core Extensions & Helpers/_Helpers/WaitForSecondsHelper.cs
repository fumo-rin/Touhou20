using UnityEngine;
using System.Collections.Generic;

namespace Core.Extensions
{
    public static partial class Helper
    {
        public static Dictionary<float, WaitForSeconds> RepeatSettingsStallLookup;
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void ResetLookup()
        {
            RepeatSettingsStallLookup = new();
        }
        public static WaitForSeconds GetWaitForSeconds(float delay)
        {
            if (RepeatSettingsStallLookup.ContainsKey(delay) && RepeatSettingsStallLookup[delay] != null)
            {
                return RepeatSettingsStallLookup[delay];
            }
            RepeatSettingsStallLookup[delay] = new WaitForSeconds(delay);
            return RepeatSettingsStallLookup[delay];
        }
    }
}
