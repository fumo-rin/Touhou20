using UnityEngine;
using UnityEngine.Events;

namespace ChurroIceDungeon
{
    public class ItemEventListener : MonoBehaviour
    {
        [SerializeField] protected DungeonUnit owner;
        [SerializeField] ItemActionKey listenerKey;
        [Header("On Use is the base call. Use an extended class and use the Payload Method for extra stuff or unit specific.")]
        [SerializeField] UnityEvent OnUse;
        private void TriggerPayload(ItemData item)
        {
            Debug.Log("Triggering Payload : " + transform.name);
            OnUse?.Invoke();
            Payload(owner);
        }
        private void Start()
        {
            ItemData.BindEvent(TriggerPayload, listenerKey);
        }
        private void OnDestroy()
        {
            ItemData.ReleaseEvent(TriggerPayload, listenerKey);
        }
        protected virtual void Payload(DungeonUnit owner)
        {

        }
    }
}
