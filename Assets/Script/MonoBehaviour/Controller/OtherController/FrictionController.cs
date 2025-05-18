using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FrictionController : MonoBehaviour
{
    private Rigidbody2D rigid;

    private void Start()
    {
        rigid = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        if (rigid.velocity.sqrMagnitude > 0)
        {
            rigid.velocity *= GameManager.setting.frictionFactor;
            if (rigid.velocity.magnitude < 0.1f)
            {
                rigid.velocity = Vector2.zero;
            }
        }
    }
}
