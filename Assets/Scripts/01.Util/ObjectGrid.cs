using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Util
{
    // 월드 좌표계를 기준으로 한다.
    public enum ObjectGridSortType
    {
        DownLeftTop,
    }
    
    
    public class ObjectGrid : MonoBehaviour
    {
        public RectOffset padding;
        public Vector3Int gridCount;
        public Vector3 cellSize;
        public Vector3 spacing;

        public ObjectGridSortType sortType;

        public void FixedUpdate()
        {
            // 일단 아래 왼쪽 위 부터 쌓인다고 가정
            var children = GetAllChildren();
            for (var i = 0; i < children.Count; i++)
            {
                var child = children[i];
                var xIndex = (i % gridCount.x);
                var yIndex = (i / (gridCount.x * gridCount.z) % gridCount.y);
                var zIndex = (i % (gridCount.x * gridCount.y) / gridCount.z);
                var x = -cellSize.x + (cellSize.x + spacing.x) * xIndex;
                var y = (cellSize.y + spacing.y) * yIndex;
                var z = cellSize.z + (-cellSize.z + spacing.z) * zIndex;
                child.localPosition = new Vector3(x, y, z);
            }
        }

        private List<Transform> GetAllChildren()
        {
            var list = new List<Transform>();
            for (int i = 0; i < transform.childCount; i++)
            {
                var t = transform.GetChild(i);
                if(t.gameObject.activeSelf)
                    list.Add(t);
            }

            return list;
        }
    }
}

