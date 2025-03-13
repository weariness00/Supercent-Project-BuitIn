using System;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

namespace Manager
{
    public class SoundBlock : MonoBehaviour
    {
        public Slider slider;

        private AudioMixerGroup _group;
        public void OnEnable()
        {
            if (!ReferenceEquals(_group, null))
            {
                float volume = SoundManager.Instance.GetVolume(_group.name);
                slider.value = volume;
            }
        }

        public virtual void Initialize(AudioMixerGroup group)
        {
            _group = group;
            float volume = SoundManager.Instance.GetVolume(group.name);
            slider.value = volume;
            slider.onValueChanged.AddListener(value => SoundManager.Instance.SetVolume(group.name, value));
            SoundManager.Instance.SetVolume(group.name, volume);
        }
    }
}

