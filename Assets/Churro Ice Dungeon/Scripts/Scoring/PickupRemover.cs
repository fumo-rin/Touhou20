using UnityEngine;

namespace ChurroIceDungeon
{
    public class PickupRemover : MonoBehaviour
    {
        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.TryGetComponent(out Pickup p))
            {
                p.ClearPickup();
            }
        }
    }
}
