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
            foreach (var bread in customer.hasBreadStack)
                moneyBundle.InstantiateMoney(bread.cellMoney);
            
            var effectAudio = SoundManager.Instance.GetEffectSource();
            effectAudio.PlayOneShot(calculateSound);
        }

        public void MessUpDiningTable()
        {
            trashObject.SetActive(true);
            chair.ChangeMessUpState();
        }

        public void CleanUpDiningTable()
        {
            if(!gameObject.activeInHierarchy) return;
            
            cleanUpEffect.Play();
            trashObject.SetActive(false);
            chair.ChangeIdleState();

            IsAvailable = true;
        }
    }

    public partial class DiningTable
    {
        [HideInInspector] public Animator animator;

        private static readonly int GoCleanUp = Animator.StringToHash("Go CleanUp");

        public void SetCleanUp() => animator.SetTrigger(GoCleanUp);
    }
}

