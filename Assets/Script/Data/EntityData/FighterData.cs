using UnityEngine;

[CreateAssetMenu(fileName = "New FighterData", menuName = "ScriptableData/FighterData")]
public class FighterData : CharacterData
{
    [Header("�������")]
    public float dashForce;
    public float dashBlank;
    public int dashMaxCount;
    public float continueDashBlank;
    [Header("��������")]
    public int attackDamage;
    public float attackStep;
    public float attackBlank;
    public int attackMaxCount;
    public float continueAttackBlank;
    public float repelForce;
    [Header("������Χ")]
    public float attackRadius;
    public float attackAngle;
    [Header("Զ������")]
    public float spellBlank;
    public float rangeSpellBlank;
    [Header("��������")]
    public int defenceDerate;
    public float defenceStep;
    public float defenceBlank;
    [Header("Ԥ��������Χ")]
    public float prepareAttackRadius;
}
