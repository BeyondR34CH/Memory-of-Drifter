using UnityEngine;

[CreateAssetMenu(fileName = "New CharacterData", menuName = "ScriptableData/CharacterData")]
public class CharacterData : ScriptableObject
{
    [Header("��������")]
    public float moveSpeed;
    public int   maxHealth;
    public Color bloodColor;
    [Header("��Ұ��Χ")]
    public float viewRadius;

    public string ToJson() => JsonUtility.ToJson(this);
}
