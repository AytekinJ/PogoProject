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

    void Update()
    {
        Animator activeAnimator = goldGfx.activeSelf ? goldAnimator : normalAnimator;

        var clipInfo = activeAnimator.GetCurrentAnimatorClipInfo(0);
        if (clipInfo.Length > 0)
        {
            currentAnimationName = clipInfo[0].clip.name;

            if (attackAnimations.Contains(currentAnimationName))
            {
                StartCoroutine(AnimationSwitchBlock(currentAnimationName));
            }
        }
    }

    bool isAnimationPlaying(Animator animator, string animationName)
    {
        var clipInfo = animator.GetCurrentAnimatorClipInfo(0);
        return clipInfo.Length > 0 && clipInfo[0].clip.name == animationName;
    }

    bool isAnimationEnded(Animator animator, string animationName)
    {
        if (!isAnimationPlaying(animator, animationName))
            return false;

        var stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        return stateInfo.normalizedTime >= 1 && !stateInfo.loop;
    }

    IEnumerator AnimationSwitchBlock(string animationName)
    {
        Animator activeAnimator = goldGfx.activeSelf ? goldAnimator : normalAnimator;

        Controller.canFlip = false;
        Controller.canChangeAnim = false;
        while (!isAnimationEnded(activeAnimator, animationName))
        {
            yield return null;
        }
        Controller.canFlip = true;
        Controller.canChangeAnim = true;
   
    }
}
