using UnityEngine;

namespace BremseTouhou
{
    [DefaultExecutionOrder(-100)]
    public class StageTarget : MonoBehaviour
    {
        public static BaseUnit TargetUnit { get; private set; }
        [SerializeField] BaseUnit unit;
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        static void Reinitialize()
        {
            TargetUnit = null;
        }
        void Awake()
        {
            TargetUnit = unit;
        }
    }
}