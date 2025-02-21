using UnityEngine;

namespace ChurroIceDungeon
{
    public class ChurroProgresser : MonoBehaviour
    {
        [SerializeField] bool startActive = false;
        [SerializeField] Collider2D progressHitbox;
        [SerializeField] bool listenToCompletionEvent;
        [SerializeField] bool gameStarter;
        int progressToLoad;
        [Header("Optional")]
        [SerializeField] GameObject visualObject;
        private void Awake()
        {
            progressHitbox.enabled = startActive;
        }
        private void Start()
        {
            progressToLoad = ChurroManager.CurrentProgress + 1;
            ChurroManager.OnDestructionRefresh += DestructionRefresh;
        }
        private void OnDestroy()
        {
            ChurroManager.OnDestructionRefresh += DestructionRefresh;
        }
        private void DestructionRefresh(float destruction, float destructionMax)
        {
            if (listenToCompletionEvent)
            {
                bool state = destruction >= destructionMax;
                if (visualObject) visualObject.SetActive(state);
                progressHitbox.enabled = state;
            }
        }
        public void TriggerProgress()
        {
            if (gameStarter)
            {
                ChurroGameProgress.StartGame();
                return;
            }
            ChurroManager.LoadProgress(progressToLoad);
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
