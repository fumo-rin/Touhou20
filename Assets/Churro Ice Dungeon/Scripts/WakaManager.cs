using UnityEngine;

namespace ChurroIceDungeon
{
    public class WakaManager : MonoBehaviour
    {
        private void FixedUpdate()
        {
            Pickup.RunPickupQueue(60);
        }
    }
}