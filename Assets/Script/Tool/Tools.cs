using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ICanInteract
{
    void Interact();
}

public static class Tools
{
    public static Vector3 RotateVector(Vector3 vector, float angle)
    {
        return Quaternion.AngleAxis(angle, Vector3.forward) * vector;
    }

    public static float Angle(Vector2 direction_1, Vector2 direction_2)
    {
        float dot = Vector2.Dot(direction_1, direction_2);
        if (dot >= 0.95f) return 0;
        else return Mathf.Acos(dot) * Mathf.Rad2Deg;
    }
}
