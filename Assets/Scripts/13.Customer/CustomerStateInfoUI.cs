using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Customer
{
    public class CustomerStateInfoUI : MonoBehaviour
    {
        public Image itemIcon;
        public TMP_Text numberText;

        public void Awake()
        {
            itemIcon.gameObject.SetActive(false);
            numberText.gameObject.SetActive(false);
        }
    }
}
