using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowTarget : MonoBehaviour
{
    [System.NonSerialized]
    public Transform target;

    private void LateUpdate()
    {
        if (target)
        {
            float x = Mathf.SmoothStep(transform.position.x, target.position.x, GameManager.setting.ronversionRate);
            float y = Mathf.SmoothStep(transform.position.y, target.position.y, GameManager.setting.ronversionRate);
            transform.position = new Vector3(x, y);
        }
    }
}
