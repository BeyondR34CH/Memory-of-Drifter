using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundEffect : MonoBehaviour
{
    public AudioSource sourec;

    private float timer;
    private Transform target;

    public void SetSound(AudioClip clip, Transform target = null)
    {
        sourec.clip = clip;
        sourec.Play();
        this.target = target;
        if (target == null) transform.localPosition = Vector3.zero;
        else transform.position = target.position;
        timer = Time.time + clip.length;
    }

    private void LateUpdate()
    {
        if (timer <= Time.time) AudioManager.PushObject(gameObject);
        if (target != null) transform.position = target.position;
    }
}
