using Core.Extensions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Bremsengine
{
    public class MusicPlayer : MonoBehaviour
    {
        public static float GlobalVolume { get; private set; }
        [SerializeField] MusicWrapper testStartingMusic;
        Queue<MusicWrapper> Playlist = new();
        [SerializeField] List<MusicWrapper> testPlaylist = new();
        private void Start()
        {
            if (testStartingMusic != null)
            {
                PlayMusicWrapper(testStartingMusic);
            }
            foreach (var item in testPlaylist)
            {
                if (item == null)
                    continue;
                Playlist.Enqueue(item);
            }
        }
        private void FixedUpdate()
        {
            if (!Application.isFocused)
                return;
            if (IsPlaying)
                return;

            if (Playlist.Count <= 0)
            {
                return;
            }
            MusicWrapper wrapper = Playlist.Dequeue();
            wrapper.Play();
        }
        private void Awake()
        {
            if (instance != null)
            {
                Destroy(gameObject);
                return;
            }
            GlobalVolume = 0.75f;
            if (track1 == null) track1 = new GameObject("Music Track 1").transform.SetParentDecorator(transform).gameObject.AddComponent<AudioSource>();
            if (track2 == null) track2 = new GameObject("Music Track 2").transform.SetParentDecorator(transform).gameObject.AddComponent<AudioSource>();
            transform.SetParent(null);
            instance = this;
            DontDestroyOnLoad(transform.gameObject);
        }
        static MusicPlayer instance;
        [SerializeField] AudioSource track1;
        [SerializeField] AudioSource track2;
        MusicWrapper song1;
        MusicWrapper song2;
        [SerializeField] float crossFadeLength = 1f;
        int selectedTrack = 0;
        public static bool IsPlaying => instance.track1.isPlaying || instance.track2.isPlaying;
        public static void PlayMusicWrapper(MusicWrapper mw)
        {
            if (instance == null)
            {
                Debug.LogError("Music Player Not Instantiated Properly");
                return;
            }
            if (mw == null)
            {
                Debug.Log("Music Wrapper is null");
                return;
            }
            instance.PlayCrossfade(mw, !IsPlaying ? 0f : instance.crossFadeLength);
        }
        private void PlayCrossfade(MusicWrapper clip, float crossfade = 0.5f)
        {
            StartCoroutine(FadeTrack(clip, crossfade));
        }
        private IEnumerator FadeTrack(MusicWrapper clip, float crossfade)
        {
            crossfade = crossfade.Max(0.00f);
            float timeElapsed = 0f;
            if (clip.musicClip == null)
            {
                Debug.Log("Missing Audio Clip");
                yield break;
            }
            if (selectedTrack != 2)
            {
                track2.clip = clip;
                track2.Play();
                song2 = clip;
                selectedTrack = 2;
                if (crossfade == 0)
                {
                    track2.volume = clip.musicVolume * GlobalVolume;
                    track1.volume = 0f;
                }
                else
                {
                    while (timeElapsed < crossfade)
                    {
                        track2.volume = Mathf.Lerp(0f, song2, timeElapsed / crossfade) * (clip.musicVolume * GlobalVolume);
                        track1.volume = Mathf.Lerp(song1, 0f, timeElapsed / crossfade);
                        timeElapsed += Time.deltaTime;
                        yield return null;
                    }
                }
                track1.Stop();
            }
            else
            {
                track1.clip = clip;
                track1.Play();
                song1 = clip;
                selectedTrack = 1;
                if (crossfade == 0)
                {
                    track1.volume = clip.musicVolume * GlobalVolume;
                    track2.volume = 0f;
                }
                else
                {
                    while (timeElapsed <= crossfade)
                    {
                        track1.volume = Mathf.Lerp(0f, song1, timeElapsed / crossfade) * (clip.musicVolume * GlobalVolume);
                        track2.volume = Mathf.Lerp(song2, 0f, timeElapsed / crossfade);
                        timeElapsed += Time.deltaTime;
                        yield return null;
                    }
                }
                track2.Stop();
            }
        }
    }
}
