using System;
using System.Collections.Generic;
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
        public AudioClip appearSound;
        private static readonly int XOffsetByHeightValue = Shader.PropertyToID("_XOffsetByHeightValue");

        public void OnDestroy()
        {
            waitingDisposable?.Dispose();
        }

        public void Appear()
        {
            appearRoom.SetActive(true);
            var sequence = DOTween.Sequence();
            var sequence1 = DOTween.Sequence();
            var sequence2 = DOTween.Sequence();
            for (int i = 0; i < appearRoom.transform.childCount; i++)
            {
                if (appearRoom.transform.GetChild(i).TryGetComponent(out MeshRenderer meshRenderer))
                {
                    foreach (var mat in meshRenderer.materials)
                    {
                        sequence1.Join(DOTween.To(() => mat.GetFloat(XOffsetByHeightValue), x => mat.SetFloat(XOffsetByHeightValue, x), 1f, 1f).SetEase(Ease.InOutSine));
                        sequence2.Join(DOTween.To(() => mat.GetFloat(XOffsetByHeightValue), x => mat.SetFloat(XOffsetByHeightValue, x), 0f, 1f).SetEase(Ease.InOutSine));
                    }
                }
            }
            sequence.Append(sequence1);
            sequence.Append(sequence2);
            sequence.Play();
            
            var effectAudio = SoundManager.Instance.GetEffectSource();
            effectAudio.PlayOneShot(appearSound);
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

