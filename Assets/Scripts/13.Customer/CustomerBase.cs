using System.Collections.Generic;
using Game;
using Game.Bread;
using Game.Dining;
using UniRx;
using UnityEngine;
using Util;

namespace Customer
{
    public enum CustomerType
    {
        TakeOut,
        Dining,
    }
    
    public partial class CustomerBase : MonoBehaviour
    {
        [HideInInspector] public NavMeshAgentUtil agent;
        [HideInInspector] public SkinnedMeshRenderer skinnedMeshRenderer;
        [HideInInspector] public CustomerSpawner spawner;

        public CustomerUI ui;
        public CustomerType type;
        [HideInInspector] public Vector3 basePosition; // 가게 이용이 완료되면 가는 장소
        public ParticleSystem happyEmojiEffect;
        
        [Tooltip("필요한 빵 랜덤 값 범위")] 
        [SerializeField] private MinMax<int> needBreadRange = new(1,3);

        [Tooltip("Dining Table 관련")] 
        public MinMaxValue<float> eatTimer = new(0, 0, 1);

        [Header("Stack 관련")] 
        public BreadContainer breadContainer;
        private Stack<BreadBase> hasBreadStack = new(10);
        public Transform stackTransform;
        private PaperBag bag;
        
        private DisplayStand targetDisplayStand;
        private Counter counter;

        [HideInInspector] public bool hasBag = false; // 포장을 가지고 있는 경우

        private void Awake()
        {
            agent = GetComponentInChildren<NavMeshAgentUtil>();
            skinnedMeshRenderer = GetComponentInChildren<SkinnedMeshRenderer>();
            animator = GetComponentInChildren<Animator>();
            ui = GetComponentInChildren<CustomerUI>();
            
            breadContainer.onAddEvent.AddListener(bread =>
            {
                hasBreadStack.Push(bread);
                bread.StopAllCoroutines();
                bread.GetBreadMove(stackTransform, new Vector3(0, (breadContainer.Count - 1) * bread.skinnedMeshRenderer.localBounds.extents.y * 2f, 0));

                if (breadContainer.Count.IsMax)
                {
                    GoCounter();
                }
            });
        }

        public void FixedUpdate()
        {
            SetMove(agent.velocity.sqrMagnitude > 0.01f);
            SetStack(hasBreadStack.Count != 0 || hasBag);
        }

        public void Init()
        {
            hasBag = false;
            type = EnumExtension.Random<CustomerType>(new []{0.8f, 0.2f});
            agent.enabled = true;

            var needBreadValue = needBreadRange.Random();
            ui.ChangeBreadState(needBreadValue);
            breadContainer.Count.Max = needBreadValue;
            
            if(bag) Destroy(bag.gameObject);

            GoDisplayStand();
            
            counter = FindObjectOfType<Counter>();
        }

        public void GoDisplayStand()
        {
            targetDisplayStand = FindObjectOfType<DisplayStand>();
            targetDisplayStand.customerContainer.TryAdd(this);
            agent.stoppingDistance = 1.5f;
        }

        public void GoCounter()
        {
            agent.stoppingDistance = 0;
            ui.ChangeCounterState();

            counter.customerContainer.TryAdd(this);
        }

        public void OutShop()
        {
            hasBreadStack.Clear();
            happyEmojiEffect.Play();
            agent.SetDestination(basePosition, () => spawner.Release(this));
        }

        public void GetBag(PaperBag _bag)
        {
            hasBag = true;
            this.bag = _bag;
        }

        public void EatFoodInDiningTable(DiningTable diningTable)
        {
            agent.enabled = false;
            eatTimer.SetMin();
            SetStartSitting();
            foreach (var bread in hasBreadStack)
                bread.gameObject.SetActive(false);
            Observable.EveryUpdate().TakeWhile(_ => !eatTimer.IsMax).Subscribe(_ => eatTimer.Current += Time.deltaTime);
            Observable.EveryUpdate().Where(_ => eatTimer.IsMax).Take(1).Subscribe(_ =>
            {
                agent.enabled = true;
                diningTable.DoneEat(this);
                SetEndSitting();
                OutShop();
            });
        }
    }

    // 애니메이션 관련
    public partial class CustomerBase
    {
        private Animator animator;
        private static readonly int IsMove = Animator.StringToHash("Is Move");
        private static readonly int HasStack = Animator.StringToHash("Has Stack");
        private static readonly int GoStartSitting = Animator.StringToHash("Go Start Sitting");
        private static readonly int GoEndSitting = Animator.StringToHash("Go End Sitting");

        public void SetMove(bool value) => animator.SetBool(IsMove, value);
        public void SetStack(bool value) => animator.SetBool(HasStack, value);
        public void SetStartSitting() => animator.SetTrigger(GoStartSitting);
        public void SetEndSitting() => animator.SetTrigger(GoEndSitting);
    }
}

