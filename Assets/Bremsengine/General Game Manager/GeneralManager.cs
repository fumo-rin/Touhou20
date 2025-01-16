using Core.Extensions;
using Core.Input;
using UnityEngine;

namespace Bremsengine
{
    #region Pause
    public partial class GeneralManager
    {
        public static bool IsPaused { get; private set; }
        private static float StoredPausedTimescale = 1f;
        public static void SetPause(bool state)
        {
            IsPaused = state;
            if (state)
            {
                //Pause
                StoredPausedTimescale = Time.timeScale;
                Time.timeScale = 0f;
                IsPaused = true;
                Debug.Log("Paused Game");
                PlayerInputController.actions.Player.Disable();
            }
            else
            {
                //Unpause
                Time.timeScale = StoredPausedTimescale;
                IsPaused = false;
                Debug.Log("Un-paused Game");
                PlayerInputController.actions.Player.Enable();
            }
        }
        public static void PauseGame()
        {
            SetPause(true);
        }
        public static void UnPauseGame()
        {
            SetPause(false);
        }
        public static void TogglePause()
        {
            SetPause(!IsPaused);
        }
    }
    #endregion
    public partial class GeneralManager : MonoBehaviour
    {
        public static GeneralManager Instance { get; private set; }
        private void Awake()
        {
            StartInstance();
        }
        private void OnDestroy()
        {
            QCHelper.ReleaseCloseAction(UnPauseGame);
            QCHelper.ReleaseOpenAction(PauseGame);
            CloseInstance();
        }
        private void Start()
        {
            QCHelper.BindOpenAction(PauseGame);
            QCHelper.BindCloseAction(UnPauseGame);
        }
        private void StartInstance()
        {
            if (Instance != null)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        private void CloseInstance()
        {
            if (Instance != this)
                return;
            Instance = null;
        }
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void ReInitialize()
        {
            Instance = null;
        }
    }
}
