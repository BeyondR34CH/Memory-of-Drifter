using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bleed : MonoBehaviour
{
    private ParticleSystem particle;
    private Transform target;

    private void Awake()
    {
        particle = transform.GetChild(0).GetComponent<ParticleSystem>();
    }

    private void OnEnable()
    {
        particle.Play();
    }

    private void Update()
    {
        if (!particle.isStopped && target) transform.position = target.position;
        else EffectPool.PushObject(gameObject);
    }

    public void SetBleed(CharacterController character)
    {
        target = character.transform;
        transform.localScale = new Vector3(character.look.x < 0 ? -1 : 1, 1, 1);
        var main = particle.main;
        main.startColor = character.bloodColor;
    }
}
