using UnityEngine;

[CreateAssetMenu(fileName = "New FighterData", menuName = "ScriptableData/FighterData")]
public class FighterData : CharacterData
{
    [Header("³å´ÌÊôĞÔ")]
    public float dashForce;
    public float dashBlank;
    public int dashMaxCount;
    public float continueDashBlank;
    [Header("¹¥»÷ÊôĞÔ")]
    public int attackDamage;
    public float attackStep;
    public float attackBlank;
    public int attackMaxCount;
    public float continueAttackBlank;
    public float repelForce;
    [Header("¹¥»÷·¶Î§")]
    public float attackRadius;
    public float attackAngle;
    [Header("Ô¶³ÌÊôĞÔ")]
    public float spellBlank;
    public float rangeSpellBlank;
    [Header("·ÀÓùÊôĞÔ")]
    public int defenceDerate;
    public float defenceStep;
    public float defenceBlank;
    [Header("Ô¤±¸¹¥»÷·¶Î§")]
    public float prepareAttackRadius;
}
