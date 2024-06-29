using System.Collections.Generic;
using UnityEngine;

public enum ItemType
{
    Use, Equip, Unable
}

[System.Serializable]
public class ItemData
{
    public int id;
    public Sprite sprite;
    public string name;
    public ItemType can;

    public float moveSpeedRate;
    public int healBuff;
    public int damageBuff;
    public int attackCountBuff;
    public int defenceBuff;
    public int dashCountBuff;
}

[CreateAssetMenu(fileName = "New ItemDataList", menuName = "ScriptableData/ItemDataList")]
public class ItemDataList : ScriptableObject
{
    public List<ItemData> list;
}
