using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LockTransTrigger : TransSceneTrigger
{
    public string needItem;

    public override void Interact()
    {
        if (InventoryManager.instance.grids.SeachItem(needItem) > 0)
        {
            base.Interact();
        }
        else
        {
            UIManager.instance.tooltip.SetTooltip($"The Door needs \"{needItem}\"");
        } 
    }
}
