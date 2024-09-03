using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Core.Extensions
{
    public struct ColorHelper
    {
        public static Color32 Peach => new Color32(235, 160, 90, 255);
    }
    public static class ColorExtensions
    {
        public static Color32 Opacity(this Color c, byte alpha = 255)
        {
            Color32 color = c;
            return new(color.r, color.g, color.b, alpha);
        }
    }
}