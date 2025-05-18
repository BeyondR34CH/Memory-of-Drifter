using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New SettingData", menuName = "ScriptableData/SettingData")]

public class SettingData : ScriptableObject
{
    [Header("��������")]
    [Tooltip("��ұ��ָ��泯��������")]
    [SerializeField]
    public bool PlayerKeepFollowLook;
    [Tooltip("ȫ��Ħ����")]
    public float frictionFactor;
    [Tooltip("��֡����")]
    public float pauseFrameRate;
    [Tooltip("��֡ʱ��")]
    public float pauseFrameTime;
    [Tooltip("������Ұ��������")]
    public float fixedDistance;
    [Tooltip("������������ʱ��")]
    public float destroyTime;
    [Tooltip("��ɫ�����ٶ�")]
    public float colorFadeSpeed;
    [Space]
    [Header("��̲�Ӱ����")]
    [Tooltip("һ�γ���ͷŲ�Ӱ������")]
    public int shadowCount;
    [Tooltip("ÿ���ͷ�ʱ����")]
    public float shadowBlank;
    [Header("��Ӱ��ɫ")]
    public Color DashShadowColor_0;
    public Color DashShadowColor_1;
    public Color DashShadowColor_2;
    [Space]
    [Header("UI")]
    [Tooltip("UI�����ٶ�")]
    public float followSpeed;
    [Tooltip("UI�任�ٶ�")]
    public float ronversionRate;
}
