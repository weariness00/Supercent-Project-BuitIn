using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Player
{
    public class PlayerUI : MonoBehaviour
    {
        [HideInInspector] public PlayerBase player;

        public TMP_Text maxText;
        
        public void Awake()
        {
            player = GetComponent<PlayerBase>();

            maxText.gameObject.SetActive(false);
        }

        public void Start()
        {
            player.hasBreadStack.onPushEvent += bread =>
            {
                if (player.hasBreadStack.IsMax)
                {
                    maxText.gameObject.SetActive(true);
                }
                else
                {
                    maxText.gameObject.SetActive(false);
                }
            };
        }
    }
}

