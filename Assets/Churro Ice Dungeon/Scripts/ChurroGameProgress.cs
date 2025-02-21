using Bremsengine;
using System.Collections.Generic;
using UnityEngine;

namespace ChurroIceDungeon
{
    public class ChurroGameProgress : MonoBehaviour
    {
        [System.Serializable]
        struct gameStage
        {
            public string name;
            public MusicWrapper stageMusic;
        }
        [SerializeField] List<gameStage> stages = new();
        public static bool GameStarted { get; private set; }
        public static bool HasLoadedProgress { get; private set; }
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Reinitialize()
        {
            HasLoadedProgress = false;
            GameStarted = false;
        }
        private void Awake()
        {
            if (HasLoadedProgress)
            {
                return;
            }
            HasLoadedProgress = true;
            ChurroManager.DefineProgress(1, () => GeneralManager.LoadSceneAfterDelay(stages[0].name, 0f));
            HashSet<string> added = new()
            {
                stages[0].name
            };
            int progress = 2;
            int musicIteration = 1;
            foreach (gameStage scene in stages)
            {
                if (added.Contains(scene.name))
                {
                    continue;
                }
                ChurroManager.DefineProgress(progress, () => GeneralManager.LoadSceneAfterDelay(scene.name, 2f));
                progress++;
                musicIteration++;
            }
        }
        public static void SetEndGame()
        {
            GameStarted = false;
            ChurroManager.LoadMainMenu(2f);
        }
        public static void StartGame()
        {
            if (!GameStarted)
            {
                ChurroManager.LoadProgress(1);
                GameStarted = true;
            }
        }
    }
}