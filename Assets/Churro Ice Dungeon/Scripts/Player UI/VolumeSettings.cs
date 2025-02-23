using Core.Extensions;
using System;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

namespace ChurroIceDungeon
{
    public class VolumeSettings : MonoBehaviour
    {
        [SerializeField] Slider effectsSlider;
        [SerializeField] Slider musicSlider;
        [SerializeField] AudioMixer[] effectsMixers;
        [SerializeField] AudioMixer[] musicMixers;
        private bool TryGetSavedValue(string key, out float value)
        {
            value = 0f;
            if (PlayerPrefs.HasKey(key))
            {
                value = PlayerPrefs.GetFloat(key);
                return true;
            }
            return false;
        }
        private void OnEnable()
        {
            effectsSlider.onValueChanged.AddListener(delegate { ReadEffectsSlider(); });
            musicSlider.onValueChanged.AddListener(delegate { ReadMusicSlider(); });
        }
        public void MainMenu()
        {
            ChurroManager.LoadProgress(0);
        }
        private void Start()
        {
            if (TryGetSavedValue("Effects", out float effectsVolume))
            {
                effectsSlider.value = effectsVolume;
                SetMixers(effectsMixers, effectsVolume);
            }
            if (TryGetSavedValue("Music", out float musicVolume))
            {
                musicSlider.value = musicVolume;
                SetMixers(musicMixers, musicVolume);
            }
        }
        private void OnDisable()
        {
            effectsSlider.onValueChanged.RemoveListener(delegate { ReadEffectsSlider(); });
            musicSlider.onValueChanged.RemoveListener(delegate { ReadMusicSlider(); });
        }
        private void StoreValue(string key, float value)
        {
            PlayerPrefs.SetFloat(key, value);
        }
        private void SetMixers(AudioMixer[] mixers, float value)
        {
            if (value < -29.5f)
            {
                value = -80f;
            }
            value = (value - 5f).Clamp(-80f, 30f);
            foreach (var item in mixers)
            {
                item.SetFloat("Volume", value);
            }
        }
        public void ReadEffectsSlider()
        {
            float value = effectsSlider.value;
            Debug.Log("value " + value);
            StoreValue("Effects", value);
            SetMixers(effectsMixers, value);
        }
        public void ReadMusicSlider()
        {
            float value = musicSlider.value;
            StoreValue("Music", value);
            SetMixers(musicMixers, value);
        }
    }
}
