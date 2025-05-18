using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EnemyStates;
using ArcherStates;

public class SkeletonArcherController : EnemyController
{
    [System.NonSerialized] public Timer spellTimer;

    protected override void Awake()
    {
        spellTimer = new Timer(data.spellBlank);
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
            targetInfo.Update();
            look = targetInfo.direction;
            AudioManager.Play(AudioType.Dash, transform);
            EffectPool.GetObject(spell).GetComponent<BulletController>().SetBullet(transform, look);
        }
    }
}
