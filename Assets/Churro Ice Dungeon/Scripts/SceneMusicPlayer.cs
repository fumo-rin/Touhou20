using Bremsengine;
using UnityEngine;

namespace ChurroIceDungeon
{
    public class SceneMusicPlayer : MonoBehaviour
    {
        [SerializeField] MusicWrapper music;
        [SerializeField] bool bypassGameStartedCheck;
        private void Start()
        {
            if (!ChurroGameProgress.GameStarted && !bypassGameStartedCheck)
                return;
            music.Play();
        }
    }
}
