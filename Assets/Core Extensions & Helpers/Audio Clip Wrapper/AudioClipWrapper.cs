using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Core.Extensions
{
    [CreateAssetMenu(fileName = "New Sound", menuName = "Audio/Audio Clip Wrapper")]
    public class AudioClipWrapper : ScriptableObject
    {
        public string SoundName = "Headhunter, Leather Belt";
        [Range(0f,1f)]
        [SerializeField] float soundVolume = 0.7f;
        [field: SerializeField] public List<ACWrapperEntry> soundClips { get; private set; } = new();
        [field: SerializeField] public bool Is3D { get; private set; }
        [ContextMenu("Play Sound")]
        public void EditorPlaySound()
        {
            this.Play(Vector2.zero);
        }
        public ACWrapperEntry[] Entries => soundClips.ToArray();
        public float GetVolume(int index)
        {
            if (soundClips[index] == null)
                return soundVolume * 0.7f;
            return soundClips[index].Volume * soundVolume;
        }
    }
    [System.Serializable]
    public class ACWrapperEntry
    {
        public AudioClip clip;
        [Range(0f,80f)]
        public float PitchVariancePercent = 5f;
        [Range(0.2f,3f)]
        public float PitchOrigin = 1f;
        [Range(0.01f,1f)]
        public float Volume = 0.7f;
    }
}
