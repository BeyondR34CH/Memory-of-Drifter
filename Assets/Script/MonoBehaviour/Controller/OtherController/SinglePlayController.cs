using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SinglePlayController : MonoBehaviour
{
    private Animator anim;

    public void SetPlay(string animName, Vector3 position)
    {
        anim.Play(animName);
        transform.position = position;
    }

    private void Awake()
    {
        anim = GetComponent<Animator>();
    }

    private void Update()
    {
        if (anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.95f) EffectPool.PushObject(gameObject);
    }
}
