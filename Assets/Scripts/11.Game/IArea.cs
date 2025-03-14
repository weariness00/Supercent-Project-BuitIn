using UnityEngine;

namespace Game
{
    public interface IArea
    {
        public GameObject Area2DObject { get; set; } // 영역을 보여주는 2D 오브젝트
        public void AreaEnter();
        public void AreaExit();
    }
}