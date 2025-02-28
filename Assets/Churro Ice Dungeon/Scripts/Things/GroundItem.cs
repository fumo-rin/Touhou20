using UnityEngine;

namespace ChurroIceDungeon
{
    public abstract class GroundItem : MonoBehaviour
    {
        [SerializeField] bool destroyOnPickup = true;
        bool pickedUp = false;
        protected abstract bool OnTouch(Collider2D other, object playerData);
        private void OnTriggerEnter2D(Collider2D collision)
        {
            bool pickedUp = false;
            if (collision.transform.GetComponent<ChurroUnit>() is ChurroUnit player)
            {
                if (OnTouch(collision, player))
                {
                    pickedUp = true;
                }
            }
            SetPickedUp(pickedUp);
        }
        private void Start()
        {
            WhenStart();
        }
        protected virtual void WhenStart()
        {

        }
        private void SetPickedUp(bool state)
        {
            pickedUp = state;
            if (pickedUp && destroyOnPickup)
            {
                Destroy(gameObject);
            }
        }
    }
}
