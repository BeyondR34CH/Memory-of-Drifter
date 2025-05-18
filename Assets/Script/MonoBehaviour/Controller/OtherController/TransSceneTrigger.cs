using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct SceneInfo
{
    public string name;
    public Vector3 position;
}

public class TransSceneTrigger : MonoBehaviour, ICanInteract
{
    public bool needInteract;
    public SceneInfo transScene;

    private bool enterTrans;

    public virtual void Interact()
    {
        if (transScene.name != "")
        {
            AudioManager.Play(AudioType.OpenDoor, transform);
            GameManager.TransitionScene(transScene);
        }
        else Debug.Log("未指定场景");
    }

    private void OnEnable() => enterTrans = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (needInteract) GameManager.instance.player.currentInteract = this;
            else if (transScene.name != "") enterTrans = true;
            else Debug.Log("未指定场景");
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            GameManager.instance.player.currentInteract = null;
        }
    }

    private void Update()
    {
        if (enterTrans) GameManager.TransitionScene(transScene);
    }
}