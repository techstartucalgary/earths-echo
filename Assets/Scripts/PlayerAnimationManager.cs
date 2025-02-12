using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimationManager : MonoBehaviour
{
    [SerializeField] Animator playerAnimator;

    public void PlayAnimation(string animationName) {
        playerAnimator.Play(animationName);
    }

    public void SetTrigger(string triggerName) {
        playerAnimator.SetTrigger(triggerName);
    }
}
