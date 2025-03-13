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
    }
}

