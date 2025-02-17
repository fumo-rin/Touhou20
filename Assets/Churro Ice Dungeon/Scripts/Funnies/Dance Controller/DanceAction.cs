using UnityEngine;

namespace ChurroIceDungeon
{
    public class DanceAction : MonoBehaviour
    {
        public void StartDance(string dance)
        {
            DanceController.StartDance(dance);
        }
    }
}
