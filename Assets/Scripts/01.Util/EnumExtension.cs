using System;
using UnityEngine;

namespace Util
{
    public static class EnumExtension
    {
        public static T Random<T>() where T : Enum
        {
            Array values = Enum.GetValues(typeof(T));
            return (T)values.GetValue(UnityEngine.Random.Range(0, values.Length));
        }
        
        public static T Random<T>(float[] proportionArray) where T : Enum
        {
            if (proportionArray == Array.Empty<float>() || proportionArray == null) return Random<T>();
            Array values = Enum.GetValues(typeof(T));

#if UNITY_EDITOR
            if(values.Length != proportionArray.Length)
                Debug.LogWarning("Random의 확률 비율의 길이가 현재 Random 범위와 다릅니다.");
#endif

            float result = UnityEngine.Random.value;
            float sum = 0;
            for (var i = 0; i < values.Length; i++)
            {
                sum += proportionArray[Mathf.Min(i, proportionArray.Length)];
                if (result <= sum)
                    return (T)values.GetValue(Mathf.Min(i, proportionArray.Length));
            }
            
            return (T)values.GetValue(UnityEngine.Random.Range(0, values.Length));
        }
    }
}

