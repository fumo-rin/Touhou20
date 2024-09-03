using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DamageNumbersPro;

namespace Core.Extensions
{
    [CreateAssetMenu(fileName ="Damage Number Wrapper", menuName ="Bremsengine/Damage Numbers Wrapper")]
    public class DamageNumberWrapper : ScriptableObject
    {
        public static implicit operator DamageNumber(DamageNumberWrapper w) => w == null ? null : w.damageNumberPrefab;
        [field: SerializeField] public int index = 0;
        [SerializeField] DamageNumber damageNumberPrefab;
    }
}
