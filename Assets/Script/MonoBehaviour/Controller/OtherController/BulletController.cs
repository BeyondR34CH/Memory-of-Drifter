using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class BulletController : MonoBehaviour
{
    public int damage;
    public float speed;
    public AudioType hitAudio;
    public string hitAnim;

    private bool isHit;
    private Transform self;
    private Transform target;
    private Rigidbody2D rigid;
    private GameObject singlePlay;

    public void SetBullet(Transform self, Vector2 direction, Transform target = null)
    {
        isHit = false;
        transform.position = self.position;
        transform.rotation = Quaternion.Euler(0, 0, Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg);
        rigid.velocity = direction * speed;
        this.self = self;
        this.target = target;
    }

    private void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        singlePlay = Resources.Load<GameObject>("Prefab/Other/SinglePlay");
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!isHit && (collision.CompareTag("Player") || collision.CompareTag("Obstacle")))
        {
            isHit = true;
            AudioManager.Play(hitAudio, self);
            collision.GetComponent<CharacterController>()?.EnterHurt(self, damage, false, 0);
            EffectPool.GetObject(singlePlay).GetComponent<SinglePlayController>().SetPlay(hitAnim, transform.position);
            EffectPool.PushObject(gameObject);
        }
    }

    private void Update()
    {
        if (target)
        {
            //в╥вы
        }
    }
}
