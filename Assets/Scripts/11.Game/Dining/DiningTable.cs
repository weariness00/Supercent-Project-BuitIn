using Customer;
using Game.Money;
using Manager;
using UnityEngine;

namespace Game.Dining
{
    public partial class DiningTable : MonoBehaviour
    {
        [Header("식사자리에서 사용하는 오브젝트들")]
        public Chair chair;
        public GameObject breadObject;
        public GameObject trashObject;
        public Transform settingTransform;
        public ParticleSystem cleanUpEffect;
        
        [Space]
        public MoneyBundle moneyBundle;
        public AudioClip calculateSound;

        [HideInInspector] public bool isMessUp;
        public bool IsAvailable { get; set; } = false;

        public void Awake()
        {
            animator = GetComponent<Animator>();
            
            breadObject.SetActive(false);
            trashObject.SetActive(false);
        }

        public void StartEat(CustomerBase customer)
        {
            breadObject.SetActive(true);
            customer.transform.position = settingTransform.position;
            customer.transform.rotation = settingTransform.rotation;
            customer.EatFoodInDiningTable(this);
        }
        
        public void DoneEat(CustomerBase customer)
        {
            breadObject.SetActive(false);
            MessUpDiningTable();

            int moneyAmount = 0;
            while (customer.hasBreadStack.TryPop(out var bread))
            {
                moneyAmount += bread.cellMoney;
                Destroy(bread.gameObject);
            }
            moneyBundle.InstantiateMoneyRange(moneyAmount * 2, 2);
            
            var effectAudio = SoundManager.Instance.GetEffectSource();
            effectAudio.PlayOneShot(calculateSound);
        }

        public void MessUpDiningTable()
        {
            isMessUp = true;
            trashObject.SetActive(true);
            chair.ChangeMessUpState();
        }

        public void CleanUpDiningTable()
        {
            if(!gameObject.activeInHierarchy || !isMessUp) return;

            cleanUpEffect.Play();
            trashObject.SetActive(false);
            chair.ChangeIdleState();

            isMessUp = false;
            IsAvailable = true;
        }
    }

    public partial class DiningTable
    {
        [HideInInspector] public Animator animator;
    }
}

