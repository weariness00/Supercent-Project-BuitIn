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
        [HideInInspector] public int needBreadValue;

        [Tooltip("Dining Table 관련")] 
        public MinMaxValue<float> eatTimer = new(0, 0, 1);

        [Header("Stack 관련")]
        public MinMaxValue<float> delayPushPop; // Stack에 Push되거나 Pop되는 딜레이
        public Transform stackTransform;
        public StackUtil<BreadBase> hasBreadStack = new(10, true);
        private PaperBag bag;
        
        private DisplayStand targetDisplayStand;
        private Counter counter;

        private bool isItemSelectionDone = false; // 아이템 선택이 끝났을 경우
        [HideInInspector] public bool hasBag = false; // 포장을 가지고 있는 경우

        private void Awake()
        {
            agent = GetComponentInChildren<NavMeshAgentUtil>();
            skinnedMeshRenderer = GetComponentInChildren<SkinnedMeshRenderer>();
            animator = GetComponentInChildren<Animator>();
            ui = GetComponentInChildren<CustomerUI>();
            
            hasBreadStack.onPushEvent += bread =>
            {
                bread.StopAllCoroutines();
                bread.GetBreadMove(stackTransform, new Vector3(0, (hasBreadStack.Count - 1) * bread.skinnedMeshRenderer.localBounds.extents.y * 2f, 0));
                if (hasBreadStack.IsMax)
                {
                    GoCounter();
                }
            };
        }

        public void FixedUpdate()
        {
            SetMove(agent.velocity.sqrMagnitude > 0.01f);
            SetStack(hasBreadStack.Count != 0 || hasBag);
        }

        public void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.layer == LayerMask.NameToLayer("Area"))
                delayPushPop.SetMin();
        }

        public void OnTriggerStay(Collider other)
        {
            if(hasBreadStack.IsMax) return;
            if (other.gameObject.layer == LayerMask.NameToLayer("Area"))
                delayPushPop.Current += Time.deltaTime;
            if (delayPushPop.IsMax)
            {
                if (other.CompareTag("Display Stand") && 
                    other.TryGetComponent(out DisplayStand displayStand) &&
                    !hasBreadStack.IsMax &&
                    displayStand.hasBreadStack.TryPeek(out var bread) &&
                    !bread.isMove)
                {
                    delayPushPop.SetMin();
                    displayStand.hasBreadStack.Pop();
                    hasBreadStack.Push(bread);
                }
            }
        }

        public void Init()
        {
            isItemSelectionDone = false;
            hasBag = false;
            type = EnumExtension.Random<CustomerType>();
            
            needBreadValue = needBreadRange.Random();
            hasBreadStack.maxValue = needBreadValue;
            ui.ChangeBreadState(needBreadValue);
            
            if(bag) Destroy(bag.gameObject);
            
            targetDisplayStand = FindObjectOfType<DisplayStand>();
            agent.enabled = true;
            agent.SetDestination(targetDisplayStand.transform.position);
            
            counter = FindObjectOfType<Counter>();
            
            agent.stoppingDistance = 1.5f;
        }

        public void GoCounter()
        {
            if (isItemSelectionDone == false)
            {
                agent.stoppingDistance = 0;
                isItemSelectionDone = true;
                ui.ChangeCounterState();
                switch (type)
                {
                    case CustomerType.TakeOut:
                        counter.hasTakeOutCustomerQueue.Enqueue(this);
                        break;
                    case CustomerType.Dining:
                        counter.hasDiningCustomerQueue.Enqueue(this);
                        break;
                }
                
            }
        }

        public void OutShop()
        {
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
                while (hasBreadStack.TryPop(out var bread))
                    Destroy(bread.gameObject);
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

