using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BremseTouhou
{
    [CreateAssetMenu(menuName ="Bremse Touhou/Stageset/Stage")]
    public class StageSO : ScriptableObject
    {
        [field: SerializeField] public List<StageSet> sets { get; private set; } = new List<StageSet>();
    }
}
