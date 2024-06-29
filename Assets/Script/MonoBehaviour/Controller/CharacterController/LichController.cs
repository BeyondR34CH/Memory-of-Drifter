using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EnemyStates;
using LichStates;

public class LichController : SkeletonArcherController
{
    [SerializeField] private Transform[] heelerPoints;
    [SerializeField] private GameObject[] heelers;

    [System.NonSerialized] public bool isRangeSpell;
    private Timer timer;
    private Collider2D[] targets;

    protected override void Awake()
    {
        timer = new Timer(data.rangeSpellBlank);
        targets = new Collider2D[10];
        base.Awake();
    }

    protected override void LoadState()
    {
        state.Relate(CharacterStateType.Idle, new Idle(this));
        state.Relate(CharacterStateType.Patrol, new Patrol(this));
        state.Relate(CharacterStateType.Chase, new LichChase(this));
        state.Relate(CharacterStateType.KeepDistance, new LichKeepDistance(this));
        state.Relate(CharacterStateType.Spell, new Spell(this));
        state.Relate(CharacterStateType.RangeSpell, new RangeSpell(this));
        state.Relate(CharacterStateType.AwaitAttack, new LichAwaitAttack(this));
        state.Relate(CharacterStateType.PrepareAttack, new PrepareAttack(this));
        state.Relate(CharacterStateType.MeleeAttack, new MeleeAttack(this));
        state.Relate(CharacterStateType.SpecialAttack, new DashAttack(this));
        state.Relate(CharacterStateType.Hurt, new Hurt(this));
        state.Relate(CharacterStateType.Death, new CommonStates.Death(this));
        state.Relate(CharacterStateType.AfterDeath, new LichAfterDeath(this));
    }

    protected override void Update()
    {
        if (timer.ReachedTime() && state.currentState is LichChase or LichKeepDistance)
        {
            timer.blank += data.rangeSpellBlank;
            timer.NextTime();
            state.Transition(CharacterStateType.RangeSpell);
        }
        base.Update();
    }

    public override void ReleaseSpell()
    {
        if (!isSpell)
        {
            isSpell = true;
            targetInfo.Update();
            look = targetInfo.direction;
            AudioManager.Play(AudioType.Spell, transform);
            EffectPool.GetObject(spell).GetComponent<BulletController>().SetBullet(transform, look);
            EffectPool.GetObject(spell).GetComponent<BulletController>().SetBullet(transform, Tools.RotateVector(look,  30));
            EffectPool.GetObject(spell).GetComponent<BulletController>().SetBullet(transform, Tools.RotateVector(look, -30));
        }
    }

    public void ReleaseRangeSpell()
    {
        if (!isRangeSpell)
        {
            isRangeSpell = true;
            AudioManager.Play(AudioType.Spell, transform);
            int targetsCount = Physics2D.OverlapCircleNonAlloc(transform.position, data.prepareAttackRadius, targets, LayerMask.GetMask("Enemy"));
            for (int i = 0; i < targetsCount; i++)
            {
                CharacterController target = targets[i].GetComponent<CharacterController>();
                if (target.currentHealth < target.maxHealth)
                {
                    target.effect.Transition(EffectStateType.Heal);
                    target.healValue = data.defenceDerate;
                    target.EnterHeal();
                }
            }
            for (int i = 0; i < heelerPoints.Length; i++)
            {
                if (!PathFinder.currentMap.HasTile(PathFinder.currentMap.WorldToCell(heelerPoints[i].position)))
                {
                    FighterController heeler = Instantiate(heelers[i]).GetComponent<FighterController>();
                    heeler.transform.position = heelerPoints[i].position;
                    heeler.effect.Transition(EffectStateType.Hurt);
                }
            }
        }
    }
}
