using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DamageNumbersPro;

namespace Core.Extensions
{
    public enum DamageNumberBindMode
    {
        None,
        ToUnit
    }
    public static class DamageNumbersProExtensions
    {
        public static DamageNumber ModifyPrefix(this DamageNumber d, string newText)
        {
            if (!string.IsNullOrEmpty(newText))
            {
                d.enableLeftText = true;
            }
            d.leftText = newText;
            return d;
        }
        public static DamageNumber ModifyScale(this DamageNumber d, float newScale)
        {
            d.SetScale(newScale);
            return d;
        }
        public static DamageNumber ModifyColor(this DamageNumber d, Color newColor) => ModifyColor(d, newColor.Opacity(255));
        public static DamageNumber ModifyColor(this DamageNumber d, Color32 newColor)
        {
            d.SetColor(newColor);
            return d;
        }
        public static DamageNumber ModifyColor(this DamageNumber d, byte r, byte g, byte b, byte a)
        {
            d.SetColor(new(r, g, b, a));
            return d;
        }
        public static DamageNumber ModifyBind(this DamageNumber d, Transform toBind)
        {
            d.followedTarget = toBind;
            return d;
        }
        public static DamageNumber ModifySortingGroup(this DamageNumber d, int group)
        {
            d.SetSpamGroup(group.ToString());
            return d;
        }
    }
}
