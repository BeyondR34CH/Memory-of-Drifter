using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DashShadow : MonoBehaviour
{
    public float keepTime;
    public float followSpeed;

    private Transform target;
    private SpriteRenderer sprite;
    private Animator anim;
    private Timer timer;

    private void Awake()
    {
        target = transform;
        sprite = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        timer = new Timer(keepTime);
    }

    private void OnEnable()
    {
        timer.NextTime();
    }

    private void LateUpdate()
    {
        if (!timer.ReachedTime())
        {
            Vector3 direction = target.position - transform.position;
            float speedRate = timer.elapsed / keepTime;
            direction.x = Mathf.Lerp(0, direction.x, followSpeed * speedRate);
            direction.y = Mathf.Lerp(0, direction.y, followSpeed * speedRate);
            transform.position += direction;
            direction.Normalize();
            anim.SetFloat("inputX", direction.x);
            anim.SetFloat("inputY", direction.y);
        }
        else EffectPool.PushObject(gameObject);
    }

    public void SetShadow(Transform target, Color color)
    {
        this.target = target;
        transform.position = target.position;
        sprite.color = color;
    }
}
