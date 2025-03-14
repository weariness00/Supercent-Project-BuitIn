using System;
using System.Collections.Generic;
using DG.Tweening;
using Player;
using UniRx;
using UnityEngine;

namespace Game.Bread
{
    public partial class GenerateBread : MonoBehaviour
    {
        public BreadSpawner breadSpawner;
        public BreadContainer breadContainer;
        public Stack<BreadBase> hasBreadStack = new(10);

        public void Awake()
        {
            var interval = breadSpawner.spawnIntervals[0] / 3;
            breadSpawner.onSpawnSuccessAction.AddListener(bread =>
            {
                DOTween.Sequence()
                    .Append(bread.transform.DOMove(bread.transform.position + bread.transform.forward * 0.5f, interval).SetEase(Ease.Linear))
                    .AppendInterval(interval)
                    .OnComplete(() =>
                    {
                        bread.rigidbody.AddForce(bread.transform.forward * 10f, ForceMode.Impulse);
                        hasBreadStack.Push(bread);
                    });
            });
            
            breadContainer.onAddEvent.AddListener(bread =>
            {
                hasBreadStack.Push(bread);
            });
            
            breadContainer.onRemoveEvent.AddListener(bread =>
            {
                breadSpawner.spawnCount.Current--;
                bread.rigidbody.isKinematic = true;
                bread.rigidbody.detectCollisions = false;
                bread.collider.enabled = false;
            });
        }

        public void Start()
        {
            breadSpawner.Play();
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

