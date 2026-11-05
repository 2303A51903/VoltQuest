using System.Collections.Generic;
using UnityEngine;

namespace BatteryRun.Audio
{
    public class SoundManager : MonoBehaviour
    {
        public static SoundManager Instance { get; private set; }

        [Header("Audio Sources")]
        public AudioSource musicSource;
        public AudioSource sfxSource;

        [Header("Clips Library")]
        public List<AudioClip> audioClips = new List<AudioClip>();

        private Dictionary<string, AudioClip> clipLookup = new Dictionary<string, AudioClip>();

        private float masterVolume = 1.0f;
        private float musicVolume = 0.8f;
        private float sfxVolume = 0.9f;
        private bool isMuted = false;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(gameObject);

            SetupAudioSources();
            LoadAudioClips();
            ApplySettings();
        }

        private void SetupAudioSources()
        {
            if (musicSource == null)
            {
                GameObject musicObj = new GameObject("MusicSource");
                musicObj.transform.SetParent(transform);
                musicSource = musicObj.AddComponent<AudioSource>();
                musicSource.loop = true;
                musicSource.playOnAwake = false;
            }

            if (sfxSource == null)
            {
                GameObject sfxObj = new GameObject("SFXSource");
                sfxObj.transform.SetParent(transform);
                sfxSource = sfxObj.AddComponent<AudioSource>();
                sfxSource.loop = false;
                sfxSource.playOnAwake = false;
            }
        }

        private void LoadAudioClips()
        {
            // Load clips passed in editor or resources
            foreach (var clip in audioClips)
            {
                if (clip != null && !clipLookup.ContainsKey(clip.name))
                {
                    clipLookup[clip.name] = clip;
                }
            }

            // Also search Resources for AudioClips
            AudioClip[] resClips = Resources.LoadAll<AudioClip>("Audio");
            foreach (var clip in resClips)
            {
                if (clip != null && !clipLookup.ContainsKey(clip.name))
                {
                    clipLookup[clip.name] = clip;
                }
                // SFX files are named SFX_Jump etc. but code requests "Jump"
                if (clip != null && clip.name.StartsWith("SFX_") && !clipLookup.ContainsKey(clip.name.Substring(4)))
                {
                    clipLookup[clip.name.Substring(4)] = clip;
                }
            }
        }

        public void RegisterClip(string name, AudioClip clip)
        {
            if (!string.IsNullOrEmpty(name) && clip != null)
            {
                clipLookup[name] = clip;
            }
        }

        public void PlayBGM(string trackName, bool loop = true)
        {
            AudioClip clip = GetOrSynthesizeClip(trackName, true);
            if (clip != null)
            {
                if (musicSource.clip == clip && musicSource.isPlaying) return;

                musicSource.clip = clip;
                musicSource.loop = loop;
                musicSource.Play();
            }
        }

        public void PlaySFX(string sfxName, float pitchRandomness = 0.05f)
        {
            if (isMuted) return;

            AudioClip clip = GetOrSynthesizeClip(sfxName, false);
            if (clip != null && sfxSource != null)
            {
                sfxSource.pitch = 1.0f + Random.Range(-pitchRandomness, pitchRandomness);
                sfxSource.PlayOneShot(clip);
            }
        }

        private AudioClip GetOrSynthesizeClip(string name, bool isMusic)
        {
            if (clipLookup.TryGetValue(name, out AudioClip clip))
            {
                return clip;
            }

            // If not found in dictionary, synthesize procedurally
            AudioClip synthesized = ProceduralAudioGenerator.GenerateProceduralClip(name, isMusic);
            if (synthesized != null)
            {
                clipLookup[name] = synthesized;
                return synthesized;
            }

            return null;
        }

        public void SetMasterVolume(float volume)
        {
            masterVolume = Mathf.Clamp01(volume);
            SaveSystem.SaveManager.SetMasterVolume(masterVolume);
            ApplyVolumes();
        }

        public void SetMusicVolume(float volume)
        {
            musicVolume = Mathf.Clamp01(volume);
            SaveSystem.SaveManager.SetMusicVolume(musicVolume);
            ApplyVolumes();
        }

        public void SetSFXVolume(float volume)
        {
            sfxVolume = Mathf.Clamp01(volume);
            SaveSystem.SaveManager.SetSFXVolume(sfxVolume);
            ApplyVolumes();
        }

        public void ToggleMute()
        {
            isMuted = !isMuted;
            SaveSystem.SaveManager.SetMuted(isMuted);
            ApplyVolumes();
        }

        public void SetMute(bool mute)
        {
            isMuted = mute;
            SaveSystem.SaveManager.SetMuted(isMuted);
            ApplyVolumes();
        }

        private void ApplySettings()
        {
            masterVolume = SaveSystem.SaveManager.GetMasterVolume();
            musicVolume = SaveSystem.SaveManager.GetMusicVolume();
            sfxVolume = SaveSystem.SaveManager.GetSFXVolume();
            isMuted = SaveSystem.SaveManager.IsMuted();
            ApplyVolumes();
        }

        private void ApplyVolumes()
        {
            if (musicSource != null)
            {
                musicSource.volume = isMuted ? 0f : (musicVolume * masterVolume);
            }
            if (sfxSource != null)
            {
                sfxSource.volume = isMuted ? 0f : (sfxVolume * masterVolume);
            }
        }

        public float MasterVolume => masterVolume;
        public float MusicVolume => musicVolume;
        public float SFXVolume => sfxVolume;
        public bool IsMuted => isMuted;
    }
}