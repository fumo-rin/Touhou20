using UnityEngine;

namespace ChurroIceDungeon
{
    [RequireComponent(typeof(CircleCollider2D))]
    public class PickupBox : MonoBehaviour
    {
        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.GetComponent<Pickup>() is Pickup p and not null)
            {
                p.StartPickup(transform);
            }
        }
    }
}
