using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DamageNumbersPro;

namespace Core.Extensions
{
    [DefaultExecutionOrder(-250)]
    public static partial class Helper
    {
        public static bool TrySpawnDamageNumber(float damage, Vector2 position, out DamageNumber n, int damageTypeIndex = -1)
        {
            n = null;
            if (damage.Floor() == 0f)
            {
                return false;
            }
            if (damageTypeIndex == -1)
            {
                damageTypeIndex = DamageNumberManager.StandardDamage;
            }
            if (DamageNumberManager.GetNumberPrefab(damageTypeIndex) is DamageNumber number and not null)
            {
                n = number.Spawn(position, damage);
                n.ModifyColor(new Color32(255, 255, 255, 255));
                n.ModifyScale(1f);
                return true;
            }
            return false;
        }
    }
    public class DamageNumberManager : MonoBehaviour
    {
        static DamageNumberManager instance;
        static Dictionary<int, DamageNumber> lookup = new Dictionary<int, DamageNumber>();
        [SerializeField] DamageNumberWrapper defaultDamage;
        [SerializeField] DamageNumberWrapper critDamage;
        public static int StandardDamage => instance == null ? 0 : instance.defaultDamage.index;
        public static int CritDamage => instance == null ? 1 : instance.critDamage.index;
        private void Awake()
        {
            instance = this;
            lookup[defaultDamage.index] = defaultDamage;
            lookup[critDamage.index] = critDamage;
        }
        public static DamageNumber GetNumberPrefab(int index)
        {
            if (lookup.ContainsKey(index))
            {
                return lookup[index];
            }
            return null;
        }
    }
}
