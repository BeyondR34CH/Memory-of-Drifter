using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EnemyStates;
using ArcherStates;

public class NecromancerController : SkeletonArcherController
{
    private Collider2D[] targets;

    protected override void Awake()
    {
        targets = new Collider2D[10];
        base.Awake();
    }

    protected override void LoadState()
    {
        state.Relate(CharacterStateType.Idle, new Idle(this));
        state.Relate(CharacterStateType.Patrol, new Patrol(this));
        state.Relate(CharacterStateType.Chase, new ArcherChase(this));
        state.Relate(CharacterStateType.KeepDistance, new ArcherKeepDistance(this));
        state.Relate(CharacterStateType.Spell, new Spell(this));
        state.Relate(CharacterStateType.AwaitAttack, new ArcherAwaitAttack(this));
        state.Relate(CharacterStateType.PrepareAttack, new ArcherPrepareAttack(this));
        state.Relate(CharacterStateType.MeleeAttack, new MeleeAttack(this));
        state.Relate(CharacterStateType.Hurt, new Hurt(this));
        state.Relate(CharacterStateType.Death, new CommonStates.Death(this));
        state.Relate(CharacterStateType.AfterDeath, new AfterDeath(this));
    }

    public override void ReleaseSpell()
    {
        if (!isSpell)
        {
            isSpell = true;
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
        }
    }
}
