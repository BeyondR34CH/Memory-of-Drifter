using UnityEngine;

[CreateAssetMenu(fileName = "New FinFunnelData", menuName = "ScriptableData/FinFunnelData")]
public class FinFunnelData : ScriptableObject
{
    [Header("»ù´¡ÊôÐÔ")]
    public float moveSpeed;
    [Header("¹¥»÷ÊôÐÔ")]
    public int attackDamage;
    [Header("¹¥»÷·¶Î§")]
    public float attackRadius;
    public float attackAngle;
    [Header("ÊÓÒ°·¶Î§")]
    public float viewRadius;
}
