using System;
using UnityEngine;
using Util;

namespace Game.Bread
{
    public class BreadContainer : MonoBehaviour
    {
        [Tooltip("빵을 얻거나 내려놓을때 딜레이")]public MinMaxValue<float> delayGetOrPut; // Stack에 Push되거나 Pop되는 딜레이
        [Tooltip("빵을 쌓아둘 Transform")] public Transform stackTransform;
        [Tooltip("빵이 저장될 공간")] public StackUtil<BreadBase> hasBreadStack = new(10, true);

        public bool HasBread => hasBreadStack.Count != 0;
        
        public void Update()
        {
            delayGetOrPut.Current += Time.deltaTime;
        }

        public void GetBread(BreadBase bread)
        {
            if(!delayGetOrPut.IsMax) return;
            var originPos = bread.transform.position;
            bread.transform.SetParent(stackTransform, false);
            bread.transform.position = originPos;
            bread.GetBreadMove(stackTransform,new Vector3(0, hasBreadStack.Count * bread.skinnedMeshRenderer.localBounds.extents.y * 2f, 0));
            
            hasBreadStack.Push(bread);
            delayGetOrPut.SetMin();
        }

        public void PutBread(out BreadBase bread)
        {
            bread = null;
            if(!delayGetOrPut.IsMax) return;

            hasBreadStack.TryPop(out bread);
            delayGetOrPut.SetMin();
        }
    }
}

