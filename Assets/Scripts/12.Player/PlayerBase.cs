using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Customer;
using Game;
using Game.Bread;
using Game.Dining;
using Game.Money;
using UnityEngine;
using UnityEngine.Serialization;
using User;
using Util;

namespace Player
{
    public class PlayerBase : MonoBehaviour
    {
        [FormerlySerializedAs("movementControl")] [HideInInspector] public PlayerMovementControl movementControlControl;
        [HideInInspector] public PlayerCameraControl cameraControl;
        [HideInInspector] public PlayerAnimatorControl animatorControl;

        [Header("Stack 관련")] 
        public MinMaxValue<float> delayPushPop; // Stack에 Push되거나 Pop되는 딜레이
        public MinMaxValue<float> moneyDelayTimer;
        public Transform stackTransform;
        public StackUtil<BreadBase> hasBreadStack = new(10, true);
        public bool HasStack => hasBreadStack.Count != 0;

        public void Awake()
        {
            movementControlControl = GetComponentInChildren<PlayerMovementControl>();
            cameraControl = GetComponentInChildren<PlayerCameraControl>();
            animatorControl = GetComponentInChildren<PlayerAnimatorControl>();

            hasBreadStack.onPushEvent += bread =>
            {
                var originPos = bread.transform.position;
                bread.transform.SetParent(stackTransform, false);
                bread.transform.position = originPos;
                bread.GetBreadMove(stackTransform,new Vector3(0, (hasBreadStack.Count - 1) * bread.skinnedMeshRenderer.localBounds.extents.y * 2f, 0));
            };
        }

        public void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.layer == LayerMask.NameToLayer("Area"))
            {
                delayPushPop.SetMin();
                moneyDelayTimer.SetMin();
            }
        }

        public void OnTriggerStay(Collider other)
        {
            if (other.gameObject.layer == LayerMask.NameToLayer("Area"))
            {
                delayPushPop.Current += Time.deltaTime;
                moneyDelayTimer.Current += Time.deltaTime;
            }
            
            if (delayPushPop.IsMax)
            {
                delayPushPop.SetMin();
                BreadBase bread;
                // 빵 생성기
                if (other.CompareTag("Generate Bread") && 
                    other.TryGetComponent(out GenerateBread generateBread) &&
                    !hasBreadStack.IsMax &&
                    generateBread.hasBreadStack.TryPop(out bread))
                {
                    hasBreadStack.Push(bread);
                }
                // 진열대
                else if (other.CompareTag("Display Stand") && 
                         other.TryGetComponent(out DisplayStand displayStand) &&
                         !displayStand.hasBreadStack.IsMax &&
                         hasBreadStack.TryPop(out bread))
                {
                    displayStand.hasBreadStack.Push(bread);
                }
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
                money.MoneyMove(transform, Vector3.up);
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
    }
}

