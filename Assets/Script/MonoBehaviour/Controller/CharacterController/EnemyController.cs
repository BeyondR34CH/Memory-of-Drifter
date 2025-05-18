using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetInfo
{
    public Vector2 localPosition;
    public float distance;
    public Vector2 direction;

    private EnemyController character;

    public TargetInfo(EnemyController character) => this.character = character;

    public void Update()
    {
        localPosition = character.target.transform.position - character.transform.position;
        distance = localPosition.magnitude;
        direction = localPosition.normalized;
    }
}

public abstract class EnemyBaseState : CharacterState<FighterData>
{
    protected EnemyController character;

    public EnemyBaseState(EnemyController character) : base(character)
    {
        this.character = character;
    }

    protected bool MoveTo(Vector3 localTarget)
    {
        if (CanSee(localTarget))
        {
            character.move = localTarget.normalized;
            character.look = localTarget.normalized;
            return true;
        }
        else
        {
            Vector2 direction = (PathFinder.GetNextNode(transform.position, transform.position + localTarget) - transform.position).normalized;
            if (direction == Vector2.zero) return false;
            else
            {
                character.move = direction;
                character.look = direction;
                return true;
            }
        }
    }
}

public abstract class EnemyController : FighterController
{
    public int itemId;
    public bool canPatrol;
    public bool canCellPartner;
    public float patrolWaitTime;
    public Transform[] patrolPoints;
    public bool canDashAttack;
    public bool canSpell;
    public GameObject spell;

    public TargetInfo targetInfo { get; protected set; }
    [NonSerialized] public bool releaseAttack;
    [NonSerialized] public bool isSpell;
    [NonSerialized] public Transform postPoints;

    protected override void Awake()
    {
        targetInfo = new TargetInfo(this);
        base.Awake();

        OnDeath += () => state.Transition(CharacterStateType.AfterDeath);
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        postPoints = new GameObject("PostPoints").transform;
        postPoints.position = transform.position;
    }

    public void ReleaseAttack() => releaseAttack = true;

    public virtual void ReleaseSpell()
    {
        if (!isSpell)
        {
            isSpell = true;
            targetInfo.Update();
            look = targetInfo.direction;
            AudioManager.Play(AudioType.Spell, transform);
            EffectPool.GetObject(spell).GetComponent<BulletController>().SetBullet(transform, look);
        }
    }
}
