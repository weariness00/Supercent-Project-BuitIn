using System;
using UniRx;
using UnityEngine;
using Util;

namespace Game.Bread
{
    public partial class GenerateBread : MonoBehaviour
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

    public partial class GenerateBread : IArea
    {
        [Header("Area 관련")]
        [SerializeField] private GameObject area2DObject;
        public GameObject Area2DObject { get; set; }
        public void AreaEnter()
        {
            area2DObject.transform.localScale = Vector3.one * 1.1f;
        }

        public void AreaExit()
        {
            area2DObject.transform.localScale = Vector3.one;
        }
    }
}

