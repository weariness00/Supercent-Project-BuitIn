using System;
using UniRx;

namespace User
{
    [Serializable]
    public class UserData
    {
        public ReactiveProperty<int> money = new(0);
    }
}