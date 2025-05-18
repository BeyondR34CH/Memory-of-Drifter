using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class ItemController : MonoBehaviour
{
    [SerializeField]
    private ItemDataList items;
    [SerializeField]
    private SpriteRenderer sprite;
    public int id;

    private void Start()
    {
        sprite.sprite = items.list[id].sprite;
    }

    private void OnDrawGizmos()
    {
        if (!Application.isPlaying) sprite.sprite = items.list[id].sprite;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (InventoryManager.instance.grids.AddItem(id))
            {
                Destroy(gameObject);
            }
        }
    }

    public void SetItem(int id, Vector3 position)
    {
        this.id = id;
        sprite.sprite = items.list[id].sprite;

        transform.position = position;
    }

    public void SetItem(ItemData data)
    {
        id = data.id;
        sprite.sprite = data.sprite;
    }
}
