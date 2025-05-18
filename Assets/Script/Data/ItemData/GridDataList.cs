using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class GridData
{
    public int id;
    public int count;

    public ItemData item => InventoryManager.instance.items.list[id];

    public void Set(int id, int count)
    {
        this.id = id;
        this.count = count;
    }

    public void Clear()
    {
        id = 0;
        count = 0;
    }

    public GridData Clone()
    {
        return MemberwiseClone() as GridData;
    }
}

[CreateAssetMenu(fileName = "New GridDataList", menuName = "ScriptableData/GridDataList")]
public class GridDataList : ScriptableObject
{
    public List<GridData> list;
    private int equipCount;

    public event Action OnUpdate;
    private int equipSize => InventoryManager.instance.equipSize;
    private ItemDataList items => InventoryManager.instance.items;
    private PlayerController player => GameManager.instance.player;

    private void SwapItem(int grid_1, int grid_2)
    {
        GridData temp = list[grid_1];
        list[grid_1] = list[grid_2];
        list[grid_2] = temp;
    }

    private void SortUpdate()
    {
        int midEnd = list.Count - equipSize;
        for (int i = 0, j = 0; i < midEnd; i++)
        {
            while (j < midEnd && list[j].count <= 0) j++;
            if (j < midEnd)
            {
                if (i != j) list[i] = list[j].Clone();
                j++;
            }
            else list[i].Clear();
        }
        for (int i = midEnd, j = midEnd; i < list.Count; i++)
        {
            while (j < list.Count && list[j].count <= 0) j++;
            if (j < list.Count)
            {
                if (i != j) list[i] = list[j].Clone();
                j++;
            }
            else list[i].Clear();
        }
        OnUpdate?.Invoke();
    }

    public void UpdateList(List<GridData> list)
    {
        equipCount = 0;
        this.list = list;
        for (int i = list.Count - equipSize; i < list.Count; i++)
        {
            if (list[i].count > 0) equipCount++;
            else break;
        }
        OnUpdate?.Invoke();
    }

    public bool AddItem(int id)
    {
        for (int i = 0; i < list.Count; i++)
        {
            if (list[i].count > 0 && list[i].id == id && items.list[id].can != ItemType.Equip)
            {
                list[i].count++;
                OnUpdate?.Invoke();
                return true;
            }
            else if (list[i].count <= 0)
            {
                list[i].Set(id, 1);
                OnUpdate?.Invoke();
                return true;
            }
        }
        return false;
    }

    public void ClearItem()
    {
        equipCount = 0;
        foreach (GridData grid in list)
        {
            grid.Clear();
        }
        OnUpdate?.Invoke();
    }

    public void ClearItem(int index)
    {
        list[index].Clear();
        SortUpdate();
    }

    public int SeachItem(string name)
    {
        foreach (GridData grid in list)
        {
            if (grid.count > 0 && grid.item.name == name)
            {
                return grid.count;
            }
        }
        return 0;
    }

    public int SeachHealItem()
    {
        foreach (GridData grid in list)
        {
            ItemData item = grid.item;
            if (grid.count > 0 && item.healBuff > 0)
            {
                grid.count--;
                SortUpdate();
                return item.healBuff;
            }
        }
        return -1;
    }

    public void UseItem(ItemData item, int index)
    {
        if (item.can == ItemType.Use && (item.healBuff <= 0 || player.currentHealth != player.data.maxHealth))
        {
            if (item.healBuff > 0) player.EnterPrepareHeal(item.healBuff);
            player.data.moveSpeed += item.moveSpeedRate;

            list[index].count--;
            SortUpdate();
        }
        else if (item.can == ItemType.Equip && equipCount < equipSize)
        {
            AudioManager.Play(AudioType.EquipItem);
            player.data.moveSpeed += item.moveSpeedRate;
            player.data.attackDamage += item.damageBuff;
            player.data.attackMaxCount += item.attackCountBuff;
            player.data.defenceDerate += item.defenceBuff;
            player.data.dashMaxCount += item.dashCountBuff;
            
            SwapItem(index, list.Count - equipSize + equipCount++);
            SortUpdate();
        }
        else
        {
            AudioManager.Play(AudioType.UnequipItem);
            InventoryManager.OnUseFail();
        }
    }

    public void Unequip(ItemData item, int index)
    {
        AudioManager.Play(AudioType.UnequipItem);
        if (AddItem(item.id))
        {
            player.data.moveSpeed -= item.moveSpeedRate;
            player.data.attackDamage -= item.damageBuff;
            player.data.attackMaxCount -= item.attackCountBuff;
            player.data.defenceDerate -= item.defenceBuff;
            player.data.dashMaxCount -= item.dashCountBuff;

            equipCount--;
            list[index].Clear();
            SortUpdate();
        }
        else InventoryManager.OnUseFail();
    }
}
