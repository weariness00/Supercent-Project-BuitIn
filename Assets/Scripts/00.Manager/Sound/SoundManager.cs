using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Audio;
using Util;

namespace Manager
{
    public class SoundManager : Singleton<SoundManager>
    {
        public SoundManagerSetting setting;
        public AudioMixer mixer => setting.mixer;
        public Canvas soundCanvas;
        public RectTransform soundCanvasRectTransform;

        private Dictionary<string, AudioSource> audioSourceDictionary = new Dictionary<string, AudioSource>();
 
        public override void Awake()
        {
            base.Awake();
            
            setting = SoundManagerSettingsProviderHelper.setting;
            Debug.Assert(setting != null, $"Sound Manager Setting 스크립터블 오브젝트가 존재하지 않습니다.");
            if(ReferenceEquals(setting, null)) return;
            
            // 본래 있던 오디오 소스 삭제
            foreach (AudioSource audioSource in audioSourceDictionary.Values)
                Destroy(audioSource.gameObject);
            audioSourceDictionary.Clear();
            var groups = mixer.FindMatchingGroups("");
            foreach (AudioMixerGroup group in groups)
            {
                AudioSourcesGenerate(group);
                if(group.name == "Master")
                    SetVolume(group.name, GetVolume(group.name));
                else
                    SetVolume(group.name, 100);
            }
        }
        
        public AudioSource AudioSourcesGenerate(AudioMixerGroup group)
        {
            GameObject obj = new GameObject { name = group.name };
            var audioSource = obj.AddComponent<AudioSource>();

            audioSource.transform.SetParent(transform);
            audioSource.playOnAwake = false;
            audioSource.outputAudioMixerGroup = group;
            if (audioSource.name == "BGM") audioSource.loop = true;

            audioSourceDictionary.TryAdd(group.name, audioSource);
            return audioSource;
        }

        public AudioSource GetAudioSource(string groupName)
        {
            audioSourceDictionary.TryGetValue(groupName, out var audioSource);
            return audioSource;
        }

        public AudioSource GetBGMSource() => GetAudioSource("BGM");
        public AudioSource GetEffectSource() => GetAudioSource("Effect");
        
        public void SetVolume(string volumeName, float value)
        {
            if(ReferenceEquals(setting, null)) return;

            setting.mixer.SetFloat(volumeName,Mathf.Clamp(value - 80f, -80f, 20f));
            PlayerPrefs.SetFloat($"{nameof(SoundManager)}{SoundExtension.Volume}{volumeName}", Mathf.Clamp(value, 0f, 100f));
        }

        public float GetVolume(string volumeName)
        {
            if(ReferenceEquals(setting, null)) return 0f;
            
            if(PlayerPrefs.HasKey($"{nameof(SoundManager)}{SoundExtension.Volume}{volumeName}"))
                return PlayerPrefs.GetFloat($"{nameof(SoundManager)}{SoundExtension.Volume}{volumeName}");
            return setting.mixer.GetFloat(volumeName, out float value) ? Mathf.Clamp(value, 0f, 100f) : 30f;
        }
    }
}
