using System;
using UnityEngine;

namespace Player
{
    public partial class PlayerAnimatorControl : MonoBehaviour
    {
        [HideInInspector] public PlayerBase player;

        public void Awake()
        {
            player = GetComponent<PlayerBase>();
            animator = GetComponentInChildren<Animator>();
        }

        public void Update()
        {
            SetIsMove(player.movementControlControl.isMove);
            SetHasStack(player.HasStack);
        }
    }

    public partial class PlayerAnimatorControl
    {
        [HideInInspector] public Animator animator;
        private static readonly int IsMove = Animator.StringToHash("IsMove");
        private static readonly int HasStack = Animator.StringToHash("Has Stack");
        public void SetIsMove(bool value) => animator.SetBool(IsMove, value);
        public void SetHasStack(bool value) => animator.SetBool(HasStack, value);
    }
}

