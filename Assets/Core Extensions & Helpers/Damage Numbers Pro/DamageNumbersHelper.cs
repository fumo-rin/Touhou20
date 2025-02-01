using DamageNumbersPro;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace Core.Extensions
{
    public static class DamageNumbersHelper
    {
        static List<DamageNumber> cache;
        public static void Populate()
        {
            void CacheDamageNumbers(IList<GameObject> damageNumbers)
            {
                foreach (var item in damageNumbers)
                {
                    if (item.GetComponent<DamageNumber>() is DamageNumber foundItem)
                    {
                        cache.Add(foundItem);
                    }
                    else
                    {
                        continue;
                    }
                }
            }
            if (cache == null)
            {
                cache = new();

                AddressablesTools.LoadKeys<GameObject>("Damage Number", CacheDamageNumbers);
            }
        }
        public static void Spawn(float damage, Vector2 position, Transform followTransform = null, int index = 0)
        {
            Populate();
            if (cache.Count > 0)
            {
                if (followTransform != null)
                {
                    cache[index % cache.Count].Spawn(position, damage, followTransform);
                }
                cache[index % cache.Count].Spawn(position, damage);
            }
        }
    }
}
