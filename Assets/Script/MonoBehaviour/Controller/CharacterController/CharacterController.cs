using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EffectState : IState
{
    protected CharacterController character;

    protected Animator anim;
    protected SpriteRenderer sprite;

    protected Action<EffectStateType> StateTransition;

    public EffectState(CharacterController character)
    {
        this.character = character;

        anim = character.GetComponent<Animator>();
        sprite = character.GetComponent<SpriteRenderer>();

        StateTransition = character.effect.Transition;
    }

    public abstract void EnterState();
    public abstract void UpdateState();
    public abstract void ExitState();
}

public abstract class CharacterState : IState
{
    protected Transform transform;
    protected Animator anim;

    protected Action<CharacterStateType> StateTransition;

    public CharacterState(CharacterController character)
    {
        transform = character.transform;
        anim = character.GetComponent<Animator>();

        StateTransition = (type) =>
        {
            //if (character.name != "Player") Debug.Log(character.name + " " + type);
            character.state.Transition(type);
        };
    }

    public abstract void EnterState();
    public abstract void UpdateState();
    public abstract void ExitState();

    protected bool AnimPlayDone(string name, float time)
    {
        AnimatorStateInfo animInfo = anim.GetCurrentAnimatorStateInfo(0);
        return animInfo.IsName(name) && animInfo.normalizedTime >= time;
    }

    protected bool CanSee(Vector2 localTarget)
    {
        return !Physics2D.Raycast(transform.position, localTarget.normalized, localTarget.magnitude, LayerMask.GetMask("Obstacle"));
    }
}

public abstract class CharacterState<Data> : CharacterState where Data : CharacterData
{
    protected Data data;

    public CharacterState(CharacterController<Data> character) : base(character)
    {
        data = character.data;
    }
}

public abstract class CharacterController : MonoBehaviour
{
    //DataInfo
    public abstract int maxHealth { get; }
    public abstract Color bloodColor { get; }
    //Info
    [NonSerialized] public int currentHealth;
    [NonSerialized] public int healValue;
    [NonSerialized] public Vector2 move;
    [NonSerialized] public Vector2 look;
    //event
    public event Action<int> OnHurt;
    public event Action OnPrepareHeal;
    public event Action<int> OnHeal;
    public event Action OnDeath;
    //Component
    [NonSerialized] public Animator anim;
    [NonSerialized] public Rigidbody2D rigid;
    //fsm
    [NonSerialized] public FiniteStateMachine<CharacterStateType> state;
    [NonSerialized] public FiniteStateMachine<EffectStateType> effect;

    protected GameObject bleedPrefab;
    protected abstract void LoadState();
    protected virtual void LoadEffect()
    {
        effect.Relate(EffectStateType.Normal, new EffectStates.Normal(this));
        effect.Relate(EffectStateType.Hurt, new EffectStates.Hurt(this));
        effect.Relate(EffectStateType.Heal, new EffectStates.Heal(this));
    }

    protected virtual void Awake()
    {
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

    public virtual void EnterHurt(Transform target, int damage, bool interrupt, float repelForce)
    {
        int trueDamage = (int)MathF.Min(damage, currentHealth);
        currentHealth -= trueDamage;
        OnHurt?.Invoke(trueDamage);

        EffectPool.GetObject(bleedPrefab).GetComponent<Bleed>().SetBleed(this);
        effect.Transition(EffectStateType.Hurt);
    }

    public virtual void EnterPrepareHeal(int value)
    {
        state.Transition(CharacterStateType.Heal);
        if (value > 0)
        {
            effect.Transition(EffectStateType.Heal);
            healValue = value;
            OnPrepareHeal?.Invoke();
        }
        else healValue = 0;
    }

    public virtual void EnterHeal()
    {
        if (healValue <= 0) return;
        int trueValue = (int)MathF.Min(healValue, maxHealth - currentHealth);
        currentHealth += trueValue;
        OnHeal?.Invoke(trueValue);
    }

    public virtual void EnterDeath() => OnDeath?.Invoke();
}

public abstract class CharacterController<Data> : CharacterController where Data : CharacterData
{
    //Data
    public Data data;
    [SerializeField] 
    protected bool showJudge;

    public override int maxHealth => data.maxHealth;
    public override Color bloodColor => data.bloodColor;

    protected virtual void OnEnable()
    {
        GetComponent<Collider2D>().enabled = true;
        currentHealth = maxHealth;
        state.Transition(CharacterStateType.Idle);
        effect.Transition(EffectStateType.Normal);
    }

    protected virtual void FixedUpdate()
    {
        //更新特效状态
        effect.currentState.UpdateState();
        //更新速度
        if (!move.Equals(Vector2.zero)) rigid.AddForce(move * data.moveSpeed);
    }

    protected virtual void Update()
    {
        //更新状态
        state.currentState.UpdateState();
        //更新动画朝向
        anim.SetFloat("InputX", look.x);
        anim.SetFloat("InputY", look.y);
    }

    protected virtual void OnDrawGizmos()
    {
        //视野范围
        Gizmos.color = Color.gray;
        Gizmos.DrawWireSphere(transform.position, data.viewRadius);
    }
}
