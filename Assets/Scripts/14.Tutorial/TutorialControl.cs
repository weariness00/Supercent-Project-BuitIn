using System;
using System.Collections;
using DG.Tweening;
using Game;
using Game.Bread;
using Game.Dining;
using Game.Money;
using Player;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using User;

namespace Tutorial
{
    public class TutorialControl : MonoBehaviour
    {
        public Tutorial2DObjectArrow Object2DArrowPrefab;
        public TutorialObjectArrow objectArrowPrefab;
        [HideInInspector] public Tutorial2DObjectArrow playerArrow;
        [HideInInspector] public TutorialObjectArrow targetArrow;

        public Image uiBlockedImagePrefab;
        private Image uiBlockedImage;
        public Transform emptyTransform;

        public void Awake()
        {
            uiBlockedImage = Instantiate(uiBlockedImagePrefab, UIManager.Instance.mainCanvas.transform);
            uiBlockedImage.gameObject.SetActive(false);
        }

        public void Start()
        {
            StartCoroutine(StartTutorial());
        }

        public void SetTargetTutorial(Transform targetTransform)
        {
            var player = FindObjectOfType<PlayerBase>();
            if (playerArrow == null) playerArrow = Instantiate(Object2DArrowPrefab);
            if (targetArrow == null) targetArrow = Instantiate(objectArrowPrefab);
            playerArrow.SetAroundMoveToDestination(targetTransform, player.transform);
            targetArrow.SetTutorialTarget(targetTransform);
        }

        public IEnumerator SetCameraMove(Transform targetTransform)
        {
            var sequence = DOTween.Sequence();
            var player = FindObjectOfType<PlayerBase>();
            var camera = player.cameraControl.camera;
            var originPos = camera.transform.position;
            player.cameraControl.enabled = false;

            bool done = false;
            sequence.Append(camera.transform.DOMoveX(targetTransform.position.x + player.cameraControl.cameraOffset.x, 1f).SetEase(Ease.InOutSine));
            sequence.Join(camera.transform.DOMoveZ(targetTransform.position.z + player.cameraControl.cameraOffset.z, 1f).SetEase(Ease.InOutSine));

            sequence.AppendInterval(1f);
            
            sequence.Append(camera.transform.DOMoveX(originPos.x, 1f).SetEase(Ease.InOutSine));
            sequence.Join(camera.transform.DOMoveZ(originPos.z, 1f).SetEase(Ease.InOutSine));

            sequence.OnComplete(() => done = true);

            EventSystem.current.sendNavigationEvents = false;
            uiBlockedImage.gameObject.SetActive(true);
            yield return new WaitUntil(() => done);
            uiBlockedImage.gameObject.SetActive(false);
            EventSystem.current.sendNavigationEvents = true;
            player.cameraControl.enabled = true;
        }

        private IEnumerator StartTutorial()
        {
            yield return GoBreadGenerateTutorial();
            yield return GoDisplayStandTutorial();
            yield return GoCounterTutorial();
            yield return GoDineInRoomTutorial();
            yield return GoEmptyTutorial();
            
            playerArrow.gameObject.SetActive(false);
            targetArrow.gameObject.SetActive(false);
        }

        private IEnumerator GoBreadGenerateTutorial()
        {
            var breadGenerate = FindObjectOfType<GenerateBread>();
            var player = FindObjectOfType<PlayerBase>();
            SetTargetTutorial(breadGenerate.transform);

            yield return new WaitUntil(() => player.breadContainer.Count != 0);
        }

        private IEnumerator GoDisplayStandTutorial()
        {
            var displayStand = FindObjectOfType<DisplayStand>();
            SetTargetTutorial(displayStand.transform);

            yield return new WaitUntil(() => displayStand.breadContainer.HasBread);
        }

        private IEnumerator GoCounterTutorial()
        {
            var counter = FindObjectOfType<Counter>();
            SetTargetTutorial(counter.transform);

            yield return new WaitUntil(() => UserManager.Instance.userData.money.Value + GameManager.Instance.droppedMoney >= 30);
        }

        private IEnumerator GoDineInRoomTutorial()
        {
            var diningRoom = FindObjectOfType<DiningRoom>();
            SetTargetTutorial(diningRoom.transform);
            yield return SetCameraMove(diningRoom.transform);
            yield return new WaitUntil(() => diningRoom.TryGetAvailableTable(out var diningTable));
        }

        private IEnumerator GoEmptyTutorial()
        {
            SetTargetTutorial(emptyTransform);
            yield return SetCameraMove(emptyTransform);

            var buyArea = emptyTransform.GetComponent<BuyArea>();
            yield return new WaitUntil(() => buyArea.needMoney.IsMax);
        }
    }
}

