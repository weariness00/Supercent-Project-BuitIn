using TMPro;
using UniRx;
using UnityEngine;
using User;

namespace Game.Money
{
    public class MoneyUI : MonoBehaviour
    {
        public Canvas mainCanvas;
        public TMP_Text amountText;

        public void Awake()
        {
            UserManager.Instance.userData.money.Subscribe(value =>
            {
                amountText.text = $"{value}";
            });
        }
    }
}

