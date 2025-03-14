using System;
using System.Linq;
using Customer;
using DG.Tweening;
using Manager;
using UniRx;
using UnityEngine;
using Util;

namespace Game.Dining
{
    public class DiningRoom : MonoBehaviour
    {
        public MovePath movePath;
        [Tooltip("사용할 수 있게 해금 되었는지")] public bool isUnlocked = false;
        public DiningTable[] diningTableArray;
        private IDisposable waitingDisposable;

        [Header("Appear 관련")] 
        public GameObject appearRoom;
        public ParticleSystem appearEffect;
        public AudioClip appearSound;

        public void OnDestroy()
        {
            waitingDisposable?.Dispose();
        }

        public void Appear()
        {
            appearRoom.SetActive(true);
            Sequence sequence = DOTween.Sequence();
            sequence.Append(transform.DORotate(new Vector3(0, 0, -45), 0.5f).SetEase(Ease.OutQuad)); // 오른쪽 45도 기울기
            sequence.Join(transform.DOScaleX(1.2f, 0.5f).SetEase(Ease.OutQuad));// 기울어지면서 가로로 늘어나기
            sequence.Append(transform.DORotate(Vector3.zero, 0.5f).SetEase(Ease.InQuad)); // 원래대로 돌아오기
            sequence.Join(transform.DOScaleX(1.0f, 0.5f).SetEase(Ease.InQuad)); // 돌아오면서 크기 복구
            sequence.Play();
            
            var effectAudio = SoundManager.Instance.GetEffectSource();
            effectAudio.PlayOneShot(appearSound);
            appearEffect.Play();
        }

        public DiningTable GetAvailableTable()
        {
            foreach (var diningTable in diningTableArray)
            {
                if (diningTable.IsAvailable)
                {
                    return diningTable;
                }
            }
            return null;
        }
        public bool TryGetAvailableTable(out DiningTable diningTable)
        {
            foreach (var dt in diningTableArray)
            {
                if (dt.IsAvailable)
                {
                    diningTable = dt;
                    return true;
                }
            }
            diningTable = null;
            return false;
        }

        public void CheckInCustomer(CustomerBase customer, Action onSeatAvailable)
        {
            if (TryGetAvailableTable(out var diningTable))
            {
                diningTable.IsAvailable = false;
                var pathList = movePath.PathPointList;
                pathList.Add(diningTable.transform.position);
                customer.ui.ChangeDisable();
                customer.agent.SetPath(pathList.ToArray(), () => diningTable.StartEat(customer));
                onSeatAvailable?.Invoke();
            }
            // 자리가 없을 경우
            else
            {
                Waiting(customer, onSeatAvailable);
            }
        }

        public void Waiting(CustomerBase customer, Action onSeatAvailable)
        {
            waitingDisposable = Observable.EveryFixedUpdate().Where(_ => TryGetAvailableTable(out var diningTable)).Take(1).Subscribe(_ =>
            {
                CheckInCustomer(customer, onSeatAvailable);
            });
        }

        public void CleanUpRoom()
        {
            foreach (var diningTable in diningTableArray)
            {
                diningTable.CleanUpDiningTable();
            }
        }
    }
}

