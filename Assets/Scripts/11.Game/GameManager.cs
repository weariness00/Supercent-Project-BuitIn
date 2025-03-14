using UnityEngine;
using Util;

namespace Game
{
    public class GameManager : Singleton<GameManager>
    {
        [Tooltip("줍지 않은 돈")] public int droppedMoney = 0;
    }
}

