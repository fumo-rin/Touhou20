using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

namespace Core.Extensions
{
    [DefaultExecutionOrder(5)]
    public static partial class AudioEngine
    {
        public static AudioMixerGroup SoundEffectsGroup { get; private set; }
        const string AudioEngineAddressableKey = "Audio Engine";
        const string AudioEngine3DPlayerName = "3D Audio Channel";
        const string AudioEngine2DPlayerName = "2D Audio Channel";
        public static AudioSource Source3D;
        public static AudioSource Source2D;
        public const int SoundChannels = 16;
        static GameObject root;
        static Queue<AudioSource> SoundQueue;
        static List<AudioSource> SoundStack;
        static AudioSource SoundIteration;
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        public static void Initialize()
        {
            SoundQueue = new();
            SoundStack = new();
            AudioSource iteration;
            root = new GameObject("Audio Engine");
            GameObject.DontDestroyOnLoad(root);
            for (int i = 0; i < SoundChannels; i++)
            {
                GameObject g = new GameObject("Channel " + i);
                g.transform.SetParent(root.transform, false);
                iteration = g.AddComponent<AudioSource>();
                iteration.playOnAwake = false;
                iteration.loop = false;

                SoundQueue.Enqueue(iteration);
                SoundStack.Add(iteration);
            }
        }
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        public static void AfterSceneLoad()
        {
            foreach (AudioMixerGroup group in AddressablesTools.LoadKeys<AudioMixerGroup>("Audio Engine"))
            {
                if (group == null)
                    continue;

                SoundEffectsGroup = group;
            }
            if (SoundEffectsGroup == null)
            {
                Debug.LogWarning("Failed to find Mixer group : Audio Engine Sound Effects");
            }
            List<AudioSource> sources = new();
            foreach (GameObject g in AddressablesTools.LoadKeys<GameObject>(AudioEngineAddressableKey))
            {
                if (g.GetComponent<AudioSource>() is AudioSource source and not null)
                {
                    if (source == null)
                        continue;
                    sources.Add(source);
                    if (source.transform.name == AudioEngine3DPlayerName)
                    {
                        Source3D = source;
                    }
                    if (source.transform.name == AudioEngine2DPlayerName)
                    {
                        Source2D = source;
                    }
                }
            }
            foreach (var channel in SoundStack)
            {
                channel.outputAudioMixerGroup = SoundEffectsGroup;
            }
        }
        private static void PlayWrapper(AudioClipWrapper a, Vector2 position)
        {
            SoundIteration = SoundQueue.Dequeue();
            SoundQueue.Enqueue(SoundIteration);

            SoundIteration.transform.position = position;
            SoundIteration.PlayWrapper(a);
        }
        public static void Play(this AudioClipWrapper a, Vector2 position)
        {
            if (a == null)
                return;
            PlayWrapper(a, position);
        }
    }
}
