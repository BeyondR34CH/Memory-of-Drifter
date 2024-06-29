using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class InventoryManager : Singleton<InventoryManager>
{
    public EventSystem eventSystem;
    public InventoryUIController playerUI;
    public GridDataList grids;
    public ItemDataList items;
    public int equipSize;

    public static event Action useFail;

    public static void OnUseFail() => useFail?.Invoke();

    protected override void Awake()
    {
        base.Awake();

        grids.OnUpdate += () => playerUI.UpdataInventory(grids);
        grids.ClearItem();
    }
}
