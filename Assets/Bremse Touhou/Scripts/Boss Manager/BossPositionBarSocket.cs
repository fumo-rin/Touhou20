using UnityEngine;

namespace BremseTouhou
{
    public class BossPositionBarSocket : MonoBehaviour
    {
        public static Transform Socket { get; private set; }
        private void Awake()
        {
            Socket = base.transform;
        }
    }
}
