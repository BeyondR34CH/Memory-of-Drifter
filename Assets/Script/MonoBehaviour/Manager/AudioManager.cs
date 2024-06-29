using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public enum AudioType
{
    Walk, OpenDoor,
    Hit, BiteHit, ArrowHit, SpellHit, Heal, Dash, 
    Attack, HeavyAttack, BiteAttack, Spell, Defence, Death,
    OpenView, CloseView, Select, FlipOver, EquipItem, UnequipItem, YouWin
}

public class AudioManager : ObjectPool<AudioManager>
{
    [Header("场景声音")]
    public AudioSource music;
    public AudioClip defaultMusic;
    public AudioClip bossMusic;
    public AudioClip endMusic;
    public AudioSource noise;
    [Header("角色音效")]
    public AudioClip[] walkOnGrass;
    public AudioClip[] walkOnStone;
    public AudioClip[] openDoor;
    public AudioClip[] hit;
    public AudioClip[] biteHit;
    public AudioClip[] arrowHit;
    public AudioClip[] spellHit;
    public AudioClip[] dash;
    public AudioClip[] attack;
    public AudioClip[] heavyAttack;
    public AudioClip[] biteAttack;
    public AudioClip[] spell;
    public AudioClip[] defence;
    public AudioClip[] death;
    [Header("界面音效")]
    public AudioClip openView;
    public AudioClip closeView;
    public AudioClip select;
    public AudioClip flipOver;
    public AudioClip heal;
    public AudioClip equipItem;
    public AudioClip unequipItem;
    public AudioClip youWin;

    public static Tilemap stoneMap { private get; set; }

    private static Transform player => GameManager.instance.player.transform;
    private static int dashNum;
    private static GameObject soundEffect;

    public static void Play(AudioType type, Transform target = null)
    {
        switch (type)
        {
            case AudioType.Walk:
                if (stoneMap) GetObject(soundEffect).GetComponent<SoundEffect>().SetSound(stoneMap.HasTile(stoneMap.WorldToCell(player.position)) ? instance.walkOnStone[Random.Range(0, instance.walkOnStone.Length - 1)] : instance.walkOnGrass[Random.Range(0, instance.walkOnGrass.Length - 1)], target);
                break;
            case AudioType.OpenDoor:
                GetObject(soundEffect).GetComponent<SoundEffect>().SetSound(instance.openDoor[Random.Range(0, instance.openDoor.Length - 1)], target);
                break;
            case AudioType.Hit:
                GetObject(soundEffect).GetComponent<SoundEffect>().SetSound(instance.hit[Random.Range(0, instance.hit.Length - 1)], target);
                break;
            case AudioType.BiteHit:
                GetObject(soundEffect).GetComponent<SoundEffect>().SetSound(instance.biteHit[Random.Range(0, instance.biteHit.Length - 1)], target);
                break;
            case AudioType.ArrowHit:
                GetObject(soundEffect).GetComponent<SoundEffect>().SetSound(instance.arrowHit[Random.Range(0, instance.arrowHit.Length - 1)], target);
                break;
            case AudioType.SpellHit:
                GetObject(soundEffect).GetComponent<SoundEffect>().SetSound(instance.spellHit[Random.Range(0, instance.spellHit.Length - 1)], target);
                break;
            case AudioType.Heal:
                GetObject(soundEffect).GetComponent<SoundEffect>().SetSound(instance.heal, target);
                break;
            case AudioType.Dash:
                GetObject(soundEffect).GetComponent<SoundEffect>().SetSound(instance.dash[dashNum++ % instance.dash.Length], target);
                break;
            case AudioType.Attack:
                GetObject(soundEffect).GetComponent<SoundEffect>().SetSound(instance.attack[Random.Range(0, instance.attack.Length - 1)], target);
                break;
            case AudioType.BiteAttack:
                GetObject(soundEffect).GetComponent<SoundEffect>().SetSound(instance.biteAttack[Random.Range(0, instance.biteAttack.Length - 1)], target);
                break;
            case AudioType.HeavyAttack:
                GetObject(soundEffect).GetComponent<SoundEffect>().SetSound(instance.heavyAttack[Random.Range(0, instance.heavyAttack.Length - 1)], target);
                break;
            case AudioType.Spell:
                GetObject(soundEffect).GetComponent<SoundEffect>().SetSound(instance.spell[Random.Range(0, instance.spell.Length - 1)], target);
                break;
            case AudioType.Defence:
                GetObject(soundEffect).GetComponent<SoundEffect>().SetSound(instance.defence[Random.Range(0, instance.defence.Length - 1)], target);
                break;
            case AudioType.Death:
                GetObject(soundEffect).GetComponent<SoundEffect>().SetSound(instance.death[Random.Range(0, instance.death.Length - 1)], target);
                break;
            case AudioType.OpenView:
                GetObject(soundEffect).GetComponent<SoundEffect>().SetSound(instance.openView, target);
                break;
            case AudioType.CloseView:
                GetObject(soundEffect).GetComponent<SoundEffect>().SetSound(instance.closeView, target);
                break;
            case AudioType.Select:
                GetObject(soundEffect).GetComponent<SoundEffect>().SetSound(instance.select, target);
                break;
            case AudioType.FlipOver:
                GetObject(soundEffect).GetComponent<SoundEffect>().SetSound(instance.flipOver, target);
                break;
            case AudioType.EquipItem:
                GetObject(soundEffect).GetComponent<SoundEffect>().SetSound(instance.equipItem, target);
                break;
            case AudioType.UnequipItem:
                GetObject(soundEffect).GetComponent<SoundEffect>().SetSound(instance.unequipItem, target);
                break;
            case AudioType.YouWin:
                GetObject(soundEffect).GetComponent<SoundEffect>().SetSound(instance.youWin, target);
                break;
            default:
                break;
        }
    }

    protected override void Awake()
    {
        base.Awake();

        dashNum = 0;
        soundEffect = Resources.Load<GameObject>("Prefab/Other/SoundEffect");
    }

    private void LateUpdate()
    {
        transform.position = player.position;
    }
}
