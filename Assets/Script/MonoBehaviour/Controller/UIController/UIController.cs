using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class UIController : MonoBehaviour
{
    public CharacterController character;
    public float YaxisOffest;

    [System.NonSerialized]
    public Transform[] cellPositions;
    protected BloodCell[] bloodCells;
    [System.NonSerialized]
    public int cellCount;
    protected Transform cellsBasePosition;
    protected const float spacing = 0.3125f;

    protected virtual void Awake()
    {
        //订阅事件
        character.OnHurt += PullCell;
        character.OnHeal += FillCell;
        character.OnDeath += CharacterDeath;
    }

    protected virtual void OnEnable()
    {
        UpdateBloodCell();
    }

    protected virtual void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(new Vector3(character.transform.position.x - 1, character.transform.position.y + YaxisOffest),
                        new Vector3(character.transform.position.x + 1, character.transform.position.y + YaxisOffest));
    }

    protected void FillCell(int num)
    {
        int currentCell = character.currentHealth - num - 1;
        for (; currentCell < character.currentHealth; currentCell++)
        {
            bloodCells[currentCell].Fill();
        }
    }

    protected void PullCell(int num)
    {
        int currentCell = character.currentHealth + num - 1;
        for (; currentCell >= character.currentHealth; currentCell--)
        {
            bloodCells[currentCell].Pull();
        }
    }

    public virtual void UpdateBloodCell()
    {
        //更新当前血量
        cellCount = character.maxHealth;
        cellPositions = new Transform[cellCount];
        bloodCells = new BloodCell[cellCount];

        //清除之前挂载在血条上的血槽位置
        if (cellsBasePosition)
        {
            foreach (Transform cell in cellsBasePosition)
                Destroy(cell.gameObject);
        }
        //若不存在则在角色上设置血条位置
        else
        {
            cellsBasePosition = new GameObject("BloodBarPosition").transform;
            cellsBasePosition.SetParent(character.transform);
            cellsBasePosition.localPosition = new Vector3(0, YaxisOffest);
        }
        //计算并设置血槽跟随位置
        float currentPosition = -(float)(cellCount - 1) / 2 * spacing;
        for (int i = 0; i < cellCount; i++)
        {
            cellPositions[i] = new GameObject("CellPosition").transform;
            cellPositions[i].SetParent(cellsBasePosition);
            cellPositions[i].localPosition = new Vector3(currentPosition, 0);
            currentPosition += spacing;
        }
        //设置血槽属性
        SetCells();
    }

    protected abstract void SetCells();

    protected abstract void CharacterDeath();
}
