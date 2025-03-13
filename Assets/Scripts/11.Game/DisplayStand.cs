using System.Collections;
using Game.Bread;
using UnityEngine;
using Util;

namespace Game
{
    public class DisplayStand : MonoBehaviour
    {
        public StackUtil<BreadBase> hasBreadStack = new(10, true);

        public Transform[] breadStandTransformArray;

        public void Awake()
        {
            hasBreadStack.onPushEvent += bread =>
            {
                bread.PutBreadMove(breadStandTransformArray[hasBreadStack.Count - 1], Vector3.zero, null);
            };
        }
    }
}

