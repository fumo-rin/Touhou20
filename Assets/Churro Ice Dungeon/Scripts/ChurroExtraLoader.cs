using Bremsengine;
using UnityEngine;

namespace ChurroIceDungeon
{
    public class ChurroExtraLoader : MonoBehaviour
    {
        [SerializeField] string sceneToLoad = "";
        [SerializeField] bool hardmode;
        private void Awake()
        {

        }
        private void SetHardMode()
        {
            NotSoGoodApple.SetMode(hardmode);
        }
        public void TriggerProgress()
        {
            GeneralManager.LoadSceneAfterDelay(sceneToLoad, 0f, SetHardMode);
        }
        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.GetComponent<ChurroUnit>() is ChurroUnit player and not null)
            {
                TriggerProgress();
            }
        }
    }
}
