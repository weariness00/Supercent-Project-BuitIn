using DG.Tweening;
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
                DOTween.To(() => 0, x =>
                {
                    amountText.text = x.ToString("N0"); // 1,000 형태로 출력
                }, value, 0.5f);
            });
        }
    }
}

