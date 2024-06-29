using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EnemyStates;
using ZombieStates;

public class ZombieController : EnemyController
{
    public bool canBite;
    public float rebornTime;

    protected override void Awake()
    {
        targetInfo = new TargetInfo(this);

        move = Vector2.zero;
        look = Vector2.down;

        anim = GetComponent<Animator>();
        rigid = GetComponent<Rigidbody2D>();

        state = new FiniteStateMachine<CharacterStateType>();
        LoadState();

        effect = new FiniteStateMachine<EffectStateType>();
        LoadEffect();

        bleedPrefab = Resources.Load<GameObject>("Prefab/Other/Bleed");
    }

    protected override void OnEnable()
    {
        state.Transition(CharacterStateType.AwaitBorn);
        effect.Transition(EffectStateType.Normal);

        postPoints = new GameObject("PostPoints").transform;
        postPoints.position = transform.position;
    }

    protected override void LoadState()
    {
        state.Relate(CharacterStateType.AwaitBorn, new ZombieAwaitBorn(this));
        state.Relate(CharacterStateType.Born, new ZombieBorn(this));
        state.Relate(CharacterStateType.Idle, new Idle(this));
        state.Relate(CharacterStateType.Patrol, new Patrol(this));
        state.Relate(CharacterStateType.Chase, new Chase(this));
        state.Relate(CharacterStateType.AwaitAttack, new AwaitAttack(this));
        state.Relate(CharacterStateType.PrepareAttack, new ZombiePrepareAttack(this));
        state.Relate(CharacterStateType.MeleeAttack, new ZombieBiteAttack(this));
        state.Relate(CharacterStateType.SpecialAttack, new DashAttack(this));
        state.Relate(CharacterStateType.Hurt, new Hurt(this));
        state.Relate(CharacterStateType.Death, new ZombieDeath(this));
        state.Relate(CharacterStateType.Reborn, new ZombieReborn(this));
    }

    public override void EnterDeath()
    {
        base.EnterDeath();
    }
}
