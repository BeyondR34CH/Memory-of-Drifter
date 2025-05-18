using UnityEngine;

[CreateAssetMenu(fileName = "New FinFunnelData", menuName = "ScriptableData/FinFunnelData")]
public class FinFunnelData : ScriptableObject
{
    [Header("��������")]
    public float moveSpeed;
    [Header("��������")]
    public int attackDamage;
    [Header("������Χ")]
    public float attackRadius;
    public float attackAngle;
    [Header("��Ұ��Χ")]
    public float viewRadius;
}
