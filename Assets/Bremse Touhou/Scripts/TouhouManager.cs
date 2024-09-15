using Core.Extensions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BremseTouhou
{
    #region Boss Kill Sound
    public partial class TouhouManager
    {
        [SerializeField] AudioClipWrapper bossKillSound;
        public static void PlayBossKillSound()
        {
            instance.bossKillSound.Play(Vector2.zero);
        }
    }
    #endregion
    public partial class TouhouManager : MonoBehaviour
    {
        static TouhouManager instance;
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Initialze()
        {
            instance = null;
        }
        private void Awake()
        {
            if (instance != null)
            {
                Destroy(gameObject);
                return;
            }
            instance = this;
            transform.SetParent(null);
            DontDestroyOnLoad(gameObject);
        }
    }
}
