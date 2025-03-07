using UnityEngine;

namespace ChurroIceDungeon
{
    public class WakaManager : MonoBehaviour
    {
        private void FixedUpdate()
        {
            Pickup.RunPickupQueue(60);
        }
        private void Awake()
        {
            Application.targetFrameRate = 120;
            QualitySettings.vSyncCount = 0;
            Time.maximumDeltaTime = 1f / 60f;
        }
    }
}