using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Test : MonoBehaviour
{
    private class t
    {
        public void a() => Debug.Log("a");
        public string b = "b";
    }
    private t tt;

    private void Awake()
    {
        Debug.Log("awake");

        tt?.a();
        Debug.Log(tt?.b);
        tt = new t();
    }

    private void Start()
    {
        Debug.Log("start");
    }

    private void OnEnable()
    {
        Debug.Log("enable");
    }
}
