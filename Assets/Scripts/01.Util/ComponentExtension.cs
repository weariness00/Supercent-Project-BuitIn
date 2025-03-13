using UnityEngine;

namespace Util
{
    public static class ComponentExtension
    {
        public static bool TryGetComponentInParent<T>(this Component c, out T component)
        {
            component = c.GetComponentInParent<T>();
            return component != null;
        }
    }
}