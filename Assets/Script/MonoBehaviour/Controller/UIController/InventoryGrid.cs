using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class InventoryGrid : MonoBehaviour, ISelectHandler, IPointerEnterHandler
{
    [SerializeField] private Image icon;
    [SerializeField] private Text count;

    public int index { get; set; }
    public ItemData item { get; private set; }

    public void Set(ItemData item, int count)
    {
        if (count > 0)
        {
            this.item = item;
            icon.sprite = item.sprite;
            icon.enabled = true;
            this.count.text = count == 1 ? "" : count.ToString();
        }
        else Clear();
    }

    public void Clear()
    {
        item = null;
        icon.enabled = false;
        count.text = "";
    }

    public void Delete()
    {
        AudioManager.Play(AudioType.CloseView);
        InventoryManager.instance.grids.ClearItem(index);
    }

    public void OnClick()
    {
        if (item != null)
        {
            if (index < 12) InventoryManager.instance.grids.UseItem(item, index);
            else            InventoryManager.instance.grids.Unequip(item, index);
        }
    }

    public void OnSelect(BaseEventData eventData) => EnterSelect();

    public void OnPointerEnter(PointerEventData eventData) => EnterSelect();

    private void EnterSelect()
    {
        InventoryManager.instance.playerUI.currentGrid = this;
        InventoryManager.instance.playerUI.EnterSelect(item);
    }
}
