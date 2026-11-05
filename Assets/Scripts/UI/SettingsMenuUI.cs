using UnityEngine;
using UnityEngine.UI;

namespace BatteryRun.UI
{
    public class SettingsMenuUI : MonoBehaviour
    {
        public Slider masterSlider;
        public Slider musicSlider;
        public Slider sfxSlider;
        public Toggle muteToggle;
        public Button closeButton;

        private void Start()
        {
            if (masterSlider != null)
            {
                masterSlider.value = SaveSystem.SaveManager.GetMasterVolume();
                masterSlider.onValueChanged.AddListener(OnMasterChanged);
            }

            if (musicSlider != null)
            {
                musicSlider.value = SaveSystem.SaveManager.GetMusicVolume();
                musicSlider.onValueChanged.AddListener(OnMusicChanged);
            }

            if (sfxSlider != null)
            {
                sfxSlider.value = SaveSystem.SaveManager.GetSFXVolume();
                sfxSlider.onValueChanged.AddListener(OnSFXChanged);
            }

            if (muteToggle != null)
            {
                muteToggle.isOn = SaveSystem.SaveManager.IsMuted();
                muteToggle.onValueChanged.AddListener(OnMuteChanged);
            }

            if (closeButton != null)
            {
                closeButton.onClick.AddListener(OnCloseClicked);
            }
        }

        private void OnMasterChanged(float val)
        {
            Audio.SoundManager.Instance?.SetMasterVolume(val);
        }

        private void OnMusicChanged(float val)
        {
            Audio.SoundManager.Instance?.SetMusicVolume(val);
        }

        private void OnSFXChanged(float val)
        {
            Audio.SoundManager.Instance?.SetSFXVolume(val);
        }

        private void OnMuteChanged(bool muted)
        {
            Audio.SoundManager.Instance?.SetMute(muted);
        }

        private void OnCloseClicked()
        {
            Audio.SoundManager.Instance?.PlaySFX("ButtonClick");
            gameObject.SetActive(false);
        }
    }
}