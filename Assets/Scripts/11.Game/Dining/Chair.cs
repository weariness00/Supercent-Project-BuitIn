using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Util;

namespace Game.Dining
{
    public class Chair : MonoBehaviour
    {
        [HideInInspector] public Animator animator;
        private float currentStateValue = 0;
        public MinMaxValue<float> stateChangeTimer = new(0,0,1);
        private static readonly int StateValue = Animator.StringToHash("State Value");

        public void Awake()
        {
            animator = GetComponentInChildren<Animator>();
        }

        public void ChangeIdleState()
        {
            StopAllCoroutines();
            StartCoroutine(ChangeStateEnumerator(0));
        }
        
        public void ChangeMessUpState()
        {
            StopAllCoroutines();
            StartCoroutine(ChangeStateEnumerator(1));
        }

        private IEnumerator ChangeStateEnumerator(int value)
        {
            stateChangeTimer.SetMin();
            float originStateValue = currentStateValue;
            while (!stateChangeTimer.IsMax)
            {
                stateChangeTimer.Current += Time.deltaTime;
                currentStateValue = Mathf.Lerp(originStateValue, value, stateChangeTimer.Current / stateChangeTimer.Max);
                animator.SetFloat(StateValue, currentStateValue);
                yield return null;
            }
        }
    }
}

