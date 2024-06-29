using UnityEngine;

[CreateAssetMenu(fileName = "New CharacterData", menuName = "ScriptableData/CharacterData")]
public class CharacterData : ScriptableObject
{
    [Header("»ù´¡ÊôÐÔ")]
    public float moveSpeed;
    public int   maxHealth;
    public Color bloodColor;
    [Header("ÊÓÒ°·¶Î§")]
    public float viewRadius;

    public string ToJson() => JsonUtility.ToJson(this);
}
