using System;
using DG.Tweening;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using User;

namespace Player
{
    public class PlayerUI : MonoBehaviour
    {
        [HideInInspector] public PlayerBase player;

        public TMP_Text moneyAmountText;
        public TMP_Text maxText;
        
        public void Awake()
        {
            player = GetComponent<PlayerBase>();
            
            maxText.gameObject.SetActive(false);
        }

        public void Start()
        {
            UserManager.Instance.userData.money.Subscribe(value =>
            {
                DOTween.To(() => value, x =>
                {
                    moneyAmountText.text = x.ToString("N0"); // 1,000 형태로 출력
                }, value, 0.5f);
            });
            player.breadContainer.onAddEvent.AddListener(bread => maxText.gameObject.SetActive(player.breadContainer.Count.IsMax));
            player.breadContainer.onRemoveEvent.AddListener(bread => maxText.gameObject.SetActive(false));
        }
    }
}

