using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Core.Extensions
{
    [CreateAssetMenu(fileName = "New Sound", menuName = "Audio/Audio Clip Wrapper")]
    public class AudioClipWrapper : ScriptableObject
    {
        [field: SerializeField] public AudioClip clip { get; private set; }
        public string SoundName = "Headhunter, Leather Belt";
        public float PitchVariance = 0.2f;
        public float PitchOrigin = 1f;
        public float Volume = 0.7f;
        [field: SerializeField] public bool Is3D { get; private set; }
    }
}
