using UnityEngine;

namespace ChurroIceDungeon
{
    public class DanceController : MonoBehaviour
    {
        static DanceController instance;
        [SerializeField] Animator anim;
        string endDanceKey = "ENDEARLY";
        private void Awake()
        {
            instance = this;
        }
        public static void CancelDance(bool state)
        {
            if (instance == null)
                return;
            instance.anim.SetBool(instance.endDanceKey, state);
        }
        public static void StartDance(string dance)
        {
            if (instance == null)
            {
                return;
            }
            if (instance.anim == null)
            {
                return;
            }
            instance.anim.SetBool(instance.endDanceKey, false);
            instance.anim.SetTrigger(dance);
        }
    }
}
