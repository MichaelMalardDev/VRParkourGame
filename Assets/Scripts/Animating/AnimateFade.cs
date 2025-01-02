using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimateFade : MonoBehaviour
{
    public Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    public void RespawnFade()
    {
        animator.SetTrigger("FadeOut");
    }

    public void OnFadeComplete()
    {
        animator.SetTrigger("FadeIn");
    }
}
