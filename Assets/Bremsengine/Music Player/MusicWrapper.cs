using Core.Extensions;
using UnityEngine;

namespace Bremsengine
{
    [CreateAssetMenu(menuName = "Bremsengine/MusicWrapper")]
    public class MusicWrapper : ScriptableObject
    {
        public static implicit operator AudioClip(MusicWrapper mw) => mw == null ? null : mw.musicClip;
        public static implicit operator float(MusicWrapper mw) => mw == null ? 0f : mw.musicVolume;
        public string TrackName = Helper.DefaultName;
        public AudioClip musicClip;
        public float musicVolume = 1f;
        private void OnValidate()
        {
            this.FindStringError(nameof(TrackName), TrackName);
        }
        public void Play()
        {
            MusicPlayer.PlayMusicWrapper(this);
        }
    }
}
