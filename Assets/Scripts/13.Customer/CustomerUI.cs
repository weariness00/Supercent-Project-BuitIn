using System.Collections.Generic;
using UnityEngine;

namespace Customer
{
    public class CustomerUI : MonoBehaviour
    {
        [HideInInspector] public CustomerBase customer;
        [Tooltip("머리위에 띄울 UI 위치")]public Transform headTransform;
        
        [Header("고객 상태를 알려주는 UI")]
        public CustomerStateInfoUI stateInfoUIPrefab;
        private CustomerStateInfoUI stateInfoUI;
        public List<Sprite> stateIconList;

        public void Awake()
        {
            customer = GetComponentInParent<CustomerBase>();
            stateInfoUI = Instantiate(stateInfoUIPrefab, UIManager.Instance.customerCanvas.transform);
            stateInfoUI.gameObject.SetActive(false);
        }

        public void LateUpdate()
        {
            stateInfoUI.transform.position = Camera.main.WorldToScreenPoint(headTransform.position);
        }

        public void ChangeBreadState(int needCount)
        {
            stateInfoUI.gameObject.SetActive(true);
            stateInfoUI.itemIcon.gameObject.SetActive(true);
            stateInfoUI.itemIcon.sprite = stateIconList[0];
            stateInfoUI.numberText.gameObject.SetActive(true);
            stateInfoUI.numberText.text = needCount.ToString();
        }

        public void ChangeCounterState()
        {
            stateInfoUI.gameObject.SetActive(true);
            stateInfoUI.itemIcon.gameObject.SetActive(true);
            stateInfoUI.itemIcon.sprite = stateIconList[1];
            stateInfoUI.numberText.gameObject.SetActive(false);
        }

        public void ChangeDiningState()
        {
            stateInfoUI.gameObject.SetActive(true);
            stateInfoUI.itemIcon.gameObject.SetActive(true);
            stateInfoUI.itemIcon.sprite = stateIconList[2];
            stateInfoUI.numberText.gameObject.SetActive(false);
        }

        public void ChangeDisable()
        {
            stateInfoUI.gameObject.SetActive(false);
            stateInfoUI.itemIcon.gameObject.SetActive(false);
            stateInfoUI.numberText.gameObject.SetActive(false);
        }
    }
}

