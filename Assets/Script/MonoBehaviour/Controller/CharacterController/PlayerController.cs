using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using PlayerStates;

public abstract class PlayerBaseState : CharacterState<FighterData>
{
    protected PlayerController character;

    public PlayerBaseState(PlayerController character) : base(character)
    {
        this.character = character;
    }

    protected Vector2 GetLookInput()
    {
        switch (GameManager.controlScheme)
        {
            case "Gamepad":
                return character.lookInput.normalized;
            case "Keyboard&Mouse":
                return ((Vector2)(GameManager.mainCamera.ScreenToWorldPoint(character.lookInput) - transform.position)).normalized;
            default:
                throw new Exception();
        }
    }
}

public class PlayerController : FighterController
{
    [SerializeField]
    private PlayerUIController playerUI;
    [NonSerialized]
    public ICanInteract currentInteract;
    //移动与朝向输入流
    public Vector2 moveInput { get; private set; }
    public Vector2 lookInput { get; private set; }
    //行动输入流
    private bool isDash;
    private bool isAttack;
    private bool isDefence;
    private bool isHeal;

    private int dashCount;
    private Timer dashTimer;
    private Timer continueDashTimer;

    private int attackCount;
    private Timer attackTimer;
    private Timer continueAttackTimer;

    private AudioSource lowHealth;

    protected override void Awake()
    {
        base.Awake();
        dashTimer = new Timer(data.dashBlank);
        continueDashTimer = new Timer(data.continueDashBlank);
        attackTimer = new Timer(data.attackBlank);
        continueAttackTimer = new Timer(data.continueAttackBlank);

        lowHealth = GetComponent<AudioSource>();

        OnDeath += GameManager.EnterGameOver;
        OnDeath += Reborn;
    }

    protected override void LoadState()
    {
        state.Relate(CharacterStateType.Idle, new Idle(this));
        state.Relate(CharacterStateType.Move, new Move(this));
        state.Relate(CharacterStateType.Dash, new Dash(this));
        state.Relate(CharacterStateType.Attack, new Attack(this));
        state.Relate(CharacterStateType.Defence, new Defence(this));
        state.Relate(CharacterStateType.Hurt, new Hurt(this));
        state.Relate(CharacterStateType.Heal, new Heal(this));
        state.Relate(CharacterStateType.Death, new CommonStates.Death(this));
    }

    protected override void LoadEffect()
    {
        base.LoadEffect();
        effect.Relate(EffectStateType.Dash, new EffectStates.Dash(this));
    }

    protected override void OnEnable()
    {
        currentHealth = maxHealth;
        state.Transition(CharacterStateType.Idle);
        effect.Transition(EffectStateType.Normal);
        dashCount = 0;
        attackCount = 0;
    }

    #region Input

    public void Move(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
    }

    public void Look(InputAction.CallbackContext context)
    {
        lookInput = context.ReadValue<Vector2>();
    }

    public void Dash(InputAction.CallbackContext context)
    {
        if (context.performed) isDash = true;
        else if (context.canceled) isDash = false;
    }

    public void Attack(InputAction.CallbackContext context)
    {
        if (context.performed) isAttack = true;
        else if (context.canceled) isAttack = false;
    }

    public void Defence(InputAction.CallbackContext context)
    {
        if (context.performed) isDefence = true; 
        else if (context.canceled) isDefence = false;
    }

    public void Heal(InputAction.CallbackContext context)
    {
        if (context.performed) isHeal = true;
        else if (context.canceled) isHeal = false;
    }

    public void Interact(InputAction.CallbackContext context)
    {
        if (context.performed) currentInteract?.Interact();
    }

    #endregion

    protected override void Update()
    {
        if (state.currentState is UnfrozenState)
        {
            if (isDash)
            {
                if (continueDashTimer.ReachedTime()) dashCount = 0;
                if (dashCount < data.dashMaxCount && dashTimer.ReachedTime())
                {
                    dashTimer.NextTime();
                    continueDashTimer.NextTime();
                    dashCount++;
                    state.Transition(CharacterStateType.Dash);
                    effect.Transition(EffectStateType.Dash);
                    isDash = false;
                }
            }
            else if (isAttack)
            {
                if (continueAttackTimer.ReachedTime()) attackCount = 0;
                if (attackCount < data.attackMaxCount && attackTimer.ReachedTime())
                {
                    attackTimer.NextTime();
                    continueAttackTimer.NextTime();
                    attackCount++;
                    state.Transition(CharacterStateType.Attack);
                    isAttack = false;
                }
            }
            else if (isDefence && data.defenceDerate > 0)
            {
                state.Transition(CharacterStateType.Defence);
                isDefence = false;
            }
            else if (isHeal && currentHealth != maxHealth)
            {
                EnterPrepareHeal(InventoryManager.instance.grids.SeachHealItem());
                isHeal = false;
            }
        }
        base.Update();
    }

    public override void EnterHurt(Transform target, int damage, bool interrupt, float repelForce)
    {
        if (state.currentState is Defence && Tools.Angle(look, (target.position - transform.position).normalized) <= data.attackAngle)
        {
            damage -= data.defenceDerate;
            AudioManager.Play(AudioType.Defence, transform);
            EnterDefence();
            rigid.AddForce(-look * repelForce, ForceMode2D.Impulse);
        }
        if (damage > 0)
        {
            base.EnterHurt(target, damage, interrupt, repelForce);
            if (currentHealth < 3) lowHealth.Play();
        }
    }

    public override void EnterHeal()
    {
        base.EnterHeal();
        if (currentHealth >= 3) lowHealth.Stop();
    }

    public void Reborn()
    {
        enabled = false;
        enabled = true;
        lowHealth.Stop();
        SaveManager.Load();
        playerUI.FillAllCells();
    }

    public void PlayWalkAudio()
    {
        AudioManager.Play(AudioType.Walk, transform);
    }
}
