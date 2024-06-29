using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCUIController : UIController
{
    private GameObject bloodCellPrefab;

    private void UnloadCell()
    {
        foreach (var cell in bloodCells)
        {
            if (cell) UIPool.PushObject(cell.gameObject);
        }
    }

    protected override void Awake()
    {
        bloodCellPrefab = Resources.Load<GameObject>("Prefab/UI/BloodCell");
        base.Awake();
    }

    protected override void OnEnable()
    {
        //GameManager.OnTransScene += OnUnload;
        base.OnEnable();
    }

    private void OnDisable() => UnloadCell();

    private void OnUnload()
    {
        GameManager.OnTransScene -= OnUnload;
        UnloadCell();
    }

    protected override void SetCells()
    {
        for (int i = 0; i < cellCount; i++)
        {
            bloodCells[i] = UIPool.GetObject(bloodCellPrefab).GetComponent<BloodCell>();

            bloodCells[i].SetCellColor(character.bloodColor);
            bloodCells[i].GetComponent<FollowTarget>().target = cellPositions[i];
            bloodCells[i].transform.position = cellPositions[i].position;
        }
    }

    protected override void CharacterDeath() => UnloadCell();
}
