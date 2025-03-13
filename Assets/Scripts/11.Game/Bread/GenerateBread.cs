using System;
using UniRx;
using UnityEngine;
using Util;

namespace Game.Bread
{
    public class GenerateBread : MonoBehaviour
    {
        public BreadSpawner breadSpawner;
        public StackUtil<BreadBase> hasBreadStack = new(10, true);

        public void Awake()
        {
            breadSpawner.onSpawnSuccessAction.AddListener(bread =>
            {
                Observable.Timer(TimeSpan.FromSeconds(0.1f)).Subscribe(_ =>
                {
                    bread.rigidbody.AddForce(bread.transform.forward * 10f, ForceMode.Impulse);
                    hasBreadStack.Push(bread);
                });
            });
            
            breadSpawner.Play();

            hasBreadStack.onPopEvent += bread =>
            {
                breadSpawner.spawnCount.Current--;
                bread.rigidbody.isKinematic = true;
                bread.rigidbody.detectCollisions = false;
                bread.collider.enabled = false;
            };
        }
    }
}

