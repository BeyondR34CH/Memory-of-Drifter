using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class InventoryUIController : MonoBehaviour
{
    public Text itemName;
    public Text currentOperation;
    [SerializeField] private Color defaultOperationColor;
    public Image hightOperationFill;
    public InventoryGrid[] inventory;
    [System.NonSerialized]
    public InventoryGrid currentGrid;
    private bool startDelete;
    private bool endDelete;
    private int targetFill;

    private void Awake()
    {
        startDelete = false;
        targetFill = -1;
        InventoryManager.useFail += UseFail;
        for (int i = 0; i < inventory.Length; i++)
        {
            inventory[i].index = i;
        }
    }

    private void Update()
    {
        if (startDelete && hightOperationFill.fillAmount >= 0.95f)
        {
            hightOperationFill.fillAmount = 1;
            startDelete = false;
            endDelete = true;
        }
        else
        {
            hightOperationFill.fillAmount += 0.015f * targetFill;
        }
    }

    private void UseFail()
    {
        StartCoroutine(FailColor());
    }

    private IEnumerator FailColor()
    {
        currentOperation.color = Color.yellow;
        yield return new WaitForSeconds(1);
        if (currentOperation.color == Color.yellow) currentOperation.color = Color.white;
    }

    public void UpdataInventory(GridDataList grids)
    {
        for (int i = 0; i < inventory.Length; i++)
        {
            inventory[i].Set(grids.list[i].item, grids.list[i].count);
        }
    }

    public void EnterSelect(ItemData item)
    {
        hightOperationFill.fillAmount = 0;
        if (item != null)
        {
            itemName.text = item.name;
            currentOperation.text = currentGrid.item.can == ItemType.Use ? "Used" : "Equipped";
            currentOperation.color = Color.white;
        }
        else
        {
            itemName.text = "Unselected";
            currentOperation.text = "Null";
            currentOperation.color = defaultOperationColor;
            targetFill = -1;
        }
    }

    public void EnterSelect(InputAction.CallbackContext context)
    {
        if (context.performed && currentGrid?.item != null)
        {
            hightOperationFill.fillAmount = 0;
            targetFill = -1;
            currentOperation.text = currentGrid.item.can == ItemType.Use ? "Used" : "Equipped";
            currentOperation.color = Color.green;
        }
        else if (context.canceled && currentOperation.color == Color.green)
        {
            currentOperation.color = Color.white;
        }
    }

    public void EnterDelete(InputAction.CallbackContext context)
    {
        if (context.performed && currentGrid?.item != null && currentGrid?.index < 12)
        {
            startDelete = true;
            //hightOperationFill.fillAmount = 0.4f;
            targetFill = 1;
            currentOperation.text = "Deleted";
            currentOperation.color = Color.white;
        }
        else if (context.canceled)
        {
            if (endDelete)
            {
                endDelete = false;
                hightOperationFill.fillAmount = 0;
                targetFill = -1;
                currentGrid.Delete();
            }
            else
            {
                targetFill = -1;
            }
        }
    }
}
