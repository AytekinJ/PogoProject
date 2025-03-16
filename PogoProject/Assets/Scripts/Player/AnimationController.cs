using System;
using System.Collections.Generic;
using UnityEngine;

public class AnimationController : MonoBehaviour
{
    [SerializeField] private GameObject goldGfx;
    [SerializeField] private GameObject normalGfx;
    [SerializeField] private Animator goldAnimator;
    [SerializeField] private Animator normalAnimator;
    [SerializeField] private List<string> attackAnimations;
    [SerializeField] string currentAnimationName;

    private void Awake()
    {
        goldAnimator = goldGfx.GetComponent<Animator>();
        normalAnimator = normalGfx.GetComponent<Animator>();
    }

    void Update()
    {
        
        if (goldGfx.activeSelf)
        {
            currentAnimationName = goldAnimator.GetCurrentAnimatorClipInfo(0)[0].clip.name;
        }
        else
        {
            currentAnimationName = normalAnimator.GetCurrentAnimatorClipInfo(0)[0].clip.name;
        }
    }

    bool isAnimationPlaying(Animator animator,string animationName)
    {   
        return animator.GetCurrentAnimatorClipInfo(0)[0].clip.name == animationName;
    }
}
