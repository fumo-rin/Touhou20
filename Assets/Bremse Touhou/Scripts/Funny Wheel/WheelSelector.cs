using UnityEngine;

namespace BremseTouhou
{
    public class WheelSelector : MonoBehaviour
    {
        [SerializeField] FunnyWheel wheel;
        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.transform.GetComponent<WheelOutcome>() is WheelOutcome w and not null)
            {
                wheel.SetLastCollision(w);
            }
        }
    }
}
