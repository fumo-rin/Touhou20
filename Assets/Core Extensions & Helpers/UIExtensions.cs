using UnityEngine;
using UnityEngine.UI;

namespace Core.Extensions
{
    public static class UIExtensions
    {
        public static Slider SetValues(this Slider s, float value, float maxValue)
        {
            s.maxValue = maxValue;
            s.value = value;
            return s;
        }
        public static Slider SetMinValue(this Slider s, float min)
        {
            s.minValue = min;
            return s;
        }
    }
}
