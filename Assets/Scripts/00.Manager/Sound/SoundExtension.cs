using UnityEngine;

namespace Manager
{
    public static class SoundExtension
    {
        public static readonly string Volume = "Volume";

        public static void SetVolume(string volumeName, float value)
        {
            PlayerPrefs.SetFloat($"{nameof(SoundManager)}{Volume}{volumeName}", Mathf.Clamp(value, 0f, 100f));
        }
    }
}