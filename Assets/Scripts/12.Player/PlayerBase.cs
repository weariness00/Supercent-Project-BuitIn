using System.Collections.Generic;
using System.Linq;
using Game;
using Game.Bread;
using Game.Dining;
using Game.Money;
using UnityEngine;
using User;
using Util;

namespace Player
{
    public class PlayerBase : MonoBehaviour
    {
        [HideInInspector] public PlayerMovementControl movementControl;
        [HideInInspector] public PlayerCameraControl cameraControl;
        [HideInInspector] public PlayerAnimatorControl animatorControl;

        [Header("Stack 관련")]
        public BreadContainer breadContainer;
        public MinMaxValue<float> moneyDelayTimer;
        public Transform stackTransform;
        private Stack<BreadBase> hasBreadStack = new(10);
        public bool HasStack => hasBreadStack.Count != 0;

        public void Awake()
        {
            movementControl = GetComponentInChildren<PlayerMovementControl>();
            cameraControl = GetComponentInChildren<PlayerCameraControl>();
            animatorControl = GetComponentInChildren<PlayerAnimatorControl>();

            breadContainer.onAddEvent.AddListener(bread =>
            {
                hasBreadStack.Push(bread);
                var originPos = bread.transform.position;
                bread.transform.SetParent(stackTransform, false);
                bread.transform.position = originPos;
                bread.GetBreadMove(stackTransform,new Vector3(0, (hasBreadStack.Count - 1) * bread.skinnedMeshRenderer.localBounds.extents.y * 2f, 0));
            });
        }

        public void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.layer == LayerMask.NameToLayer("Area"))
            {
                moneyDelayTimer.SetMin();
                if (other.TryGetComponent(out IArea area))
                    area.AreaEnter();
            }
        }

        public void OnTriggerStay(Collider other)
        {
            if (other.gameObject.layer == LayerMask.NameToLayer("Area"))
            {
                moneyDelayTimer.Current += Time.deltaTime;
            }
            
            BreadBase bread;
            // 빵 생성기
            if (other.CompareTag("Generate Bread") && 
                other.TryGetComponent(out GenerateBread generateBread) &&
                generateBread.hasBreadStack.TryPeek(out bread) &&
                breadContainer.TryAdd(bread))
            {
                generateBread.hasBreadStack.Pop();
                generateBread.breadContainer.Remove(bread);
            }
            // 진열대
            else if (other.CompareTag("Display Stand") && 
                     other.TryGetComponent(out DisplayStand displayStand) &&
                     hasBreadStack.TryPeek(out bread) &&
                     displayStand.breadContainer.TryAdd(bread))
            {
                hasBreadStack.Pop();
                breadContainer.Remove(bread);
            }
            
            if (moneyDelayTimer.IsMax &&
                other.CompareTag("Money Bundle") &&
                other.TryGetComponentInParent(out MoneyBundle moneyBundle) && 
                moneyBundle.HasMoney)
            {
                moneyDelayTimer.SetMin();
                var m = moneyBundle.activeMoneyList.FirstOrDefault();
                var money = Instantiate(m, moneyBundle.transform.position, Quaternion.identity);
                money.transform.localScale = m.transform.lossyScale;
                money.GetMoneyMove(transform, Vector3.up);
                UserManager.Instance.userData.money.Value += money.moneyAmount;
                moneyBundle.moneyPool.Release(m);
            }

            if (other.CompareTag("Counter") &&
                other.TryGetComponentInParent(out Counter counter))
            {
                counter.Calculate();
            }

            if (other.CompareTag("Dining Room") &&
                other.TryGetComponent(out DiningRoom diningRoom))
            {
                diningRoom.CleanUpRoom();
            }
        }
        
        public void OnTriggerExit(Collider other)
        {
            if (other.gameObject.layer == LayerMask.NameToLayer("Area"))
            {
                if (other.TryGetComponent(out IArea area))
                    area.AreaExit();
            }
        }
    }
}

