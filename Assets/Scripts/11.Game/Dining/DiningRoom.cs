using System;
using System.Linq;
using Customer;
using UniRx;
using UnityEngine;
using Util;

namespace Game.Dining
{
    public class DiningRoom : MonoBehaviour
    {
        public MovePath movePath;
        [Tooltip("사용할 수 있게 해금 되었는지")]public bool isUnlocked = false;
        public DiningTable[] diningTableArray;
        private IDisposable waitingDisposable;

        public void OnDestroy()
        {
            waitingDisposable?.Dispose();
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

