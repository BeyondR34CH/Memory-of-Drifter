using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IState
{
    void EnterState();
    void UpdateState();
    void ExitState();
}

public enum CharacterStateType
{
    AwaitBorn, Born, Idle, 
    Move, Dash, Patrol, Chase, KeepDistance,
    Attack, AwaitAttack, PrepareAttack, MeleeAttack, RangedAttack, SpecialAttack, 
    Spell, RangeSpell,
    Defence, 
    Hurt, Heal, Death, AfterDeath, Reborn
}

public enum EffectStateType
{
    Normal, Hurt, Heal, Defence, Dash
}

public enum PlayerUIStateType
{
    Normal, Option, Map, Inventory
}

public enum GameEndType
{
    Ready, Start, End
}

public class FiniteStateMachine<StateType> where StateType : System.Enum
{
    public IState currentState { get; private set; }

    private Dictionary<StateType, IState> states = new Dictionary<StateType, IState>();

    public void Relate(StateType type, IState state)
    {
        if (!states.ContainsKey(type)) states.Add(type, state);
    }

    public void Clear() => states.Clear();

    public void Transition(StateType type)
    {
        if (states.ContainsKey(type))
        {
            currentState?.ExitState();
            currentState = states[type];
            currentState.EnterState();
        }
        else Debug.Log("Î´°ó¶¨Ä¿±ê×´Ì¬: " + type);
    }
}
