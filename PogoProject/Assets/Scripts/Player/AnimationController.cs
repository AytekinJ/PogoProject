using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationController : MonoBehaviour
{
    [SerializeField] private GameObject goldGfx;
    [SerializeField] private GameObject normalGfx;
    [SerializeField] private Animator goldAnimator;
    [SerializeField] private Animator normalAnimator;
    [SerializeField] private List<string> attackAnimations;
    [SerializeField] private string currentAnimationName;
    private bool isBlocking = false;

    void Update()
    {
        Animator activeAnimator = goldGfx.activeSelf ? goldAnimator : normalAnimator;

        var stateInfo = activeAnimator.GetCurrentAnimatorStateInfo(0);
        if (!isBlocking)
        {
            currentAnimationName = activeAnimator.GetCurrentAnimatorClipInfo(0)[0].clip.name;

            if (attackAnimations.Contains(currentAnimationName))
            {
                StartCoroutine(AnimationSwitchBlock(currentAnimationName));
            }
        }
    }

    bool isAnimationPlaying(Animator animator, string animationName)
    {
        var stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        return stateInfo.IsName(animationName);
    }

    bool isAnimationEnded(Animator animator, string animationName)
    {
        if (!isAnimationPlaying(animator, animationName))
            return false;

        var stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        return stateInfo.normalizedTime >= 1.0f && !stateInfo.loop;
    }

    IEnumerator AnimationSwitchBlock(string animationName)
    {
        Debug.Log(animationName+" is blocking");
        Animator activeAnimator = goldGfx.activeSelf ? goldAnimator : normalAnimator;

        Controller.canFlip = false;
        Controller.canChangeAnim = false;
        isBlocking = true;

        while (!isAnimationEnded(activeAnimator, animationName) && isAnimationPlaying(activeAnimator, animationName))
        {
            yield return null;
        }

        isBlocking = false;
        Controller.canFlip = true;
        Controller.canChangeAnim = true;
        Debug.Log(animationName+" released");
    }
}