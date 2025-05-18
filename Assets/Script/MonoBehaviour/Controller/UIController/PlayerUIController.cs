using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using PlayerUIStates;

public abstract class PlayerUIState : IState
{
    protected PlayerUIController controller;
    protected Canvas canvas;
    protected FollowTarget[] panelFollow;

    protected Transform[] defaultPositions;
    protected Transform[] currentPositions;

    protected const float rate = 0.0625f;

    protected PlayerUIState(PlayerUIController controller)
    {
        this.controller = controller;
        canvas = controller.GetComponent<Canvas>();
        panelFollow = controller.panelFollow;

        defaultPositions = controller.cellPositions;
        currentPositions = controller.currentPositions;
    }

    public abstract void EnterState();
    public abstract void UpdateState();
    public abstract void ExitState();
}

public class PlayerUIController : UIController
{
    public GameObject optionEnter;
    public GameObject inventoryEnter;
    public BasePanel[] panels;

    [System.NonSerialized]
    public FollowTarget[] panelFollow;
    [System.NonSerialized]
    public Transform[] currentPositions;
    [System.NonSerialized]
    public FiniteStateMachine<PlayerUIStateType> state;

    private LRPanel leftPanel;
    private LRPanel rightPanel;

    private void Start()
    {
        state = new FiniteStateMachine<PlayerUIStateType>();
        state.Relate(PlayerUIStateType.Normal, new Normal(this));
        state.Relate(PlayerUIStateType.Option, new Option(this));
        state.Relate(PlayerUIStateType.Map, new Map(this));
        state.Relate(PlayerUIStateType.Inventory, new Inventory(this));
        state.Transition(PlayerUIStateType.Normal);
    }

    protected override void SetCells()
    {
        leftPanel = panels[1] as LRPanel;
        rightPanel = panels[3] as LRPanel;

        panelFollow = new FollowTarget[cellCount];
        for (int i = 0; i < cellCount; i++)
        {
            bloodCells[i] = panels[i].cell;
            bloodCells[i].SetCellColor(character.bloodColor);

            panelFollow[i] = panels[i].GetComponent<FollowTarget>();
            panelFollow[i].target = cellPositions[i];

            panels[i].transform.position = cellPositions[i].position;
        }
    }

    public override void UpdateBloodCell()
    {
        base.UpdateBloodCell();
        currentPositions = new Transform[cellCount];
        for (int i = 0; i < cellCount; i++)
        {
            currentPositions[i] = new GameObject("PlayerUIPosition").transform;
            currentPositions[i].SetParent(cellsBasePosition);
            currentPositions[i].localPosition = cellPositions[i].localPosition;
        }
    }

    protected override void CharacterDeath()
    {
        if (state.currentState is not Normal) state.Transition(PlayerUIStateType.Normal);
    }

    public void FillAllCells()
    {
        foreach (BloodCell cell in bloodCells)
        {
            cell.Fill();
        }
    }

    #region Input

    public void EnterNavigate(InputAction.CallbackContext context)
    {
        if (context.performed) AudioManager.Play(AudioType.Select);
    }

    public void EnterPause(InputAction.CallbackContext context)
    {
        if (context.canceled)
        {
            if (state.currentState is Normal)
            {
                state.Transition(PlayerUIStateType.Option);
            }
        }
    }

    public void EnterMap(InputAction.CallbackContext context)
    {
        if (context.canceled)
        {
            if (state.currentState is Normal)
            {
                state.Transition(PlayerUIStateType.Map);
            }
            else if (state.currentState is Map)
            {
                state.Transition(PlayerUIStateType.Normal);
            }
        }
    }

    public void EnterInventory(InputAction.CallbackContext context)
    {
        if (context.canceled)
        {
            if (state.currentState is Normal)
            {
                state.Transition(PlayerUIStateType.Inventory);
            }
            else if (state.currentState is Inventory)
            {
                state.Transition(PlayerUIStateType.Normal);
            }
        }
    }

    public void SwitchLeft(InputAction.CallbackContext context)
    {
        if (context.canceled)
        {
            AudioManager.Play(AudioType.FlipOver);
            if (state.currentState is Map)
            {
                state.Transition(PlayerUIStateType.Option);
                leftPanel.SwitchRight();
            }
            else if (state.currentState is Inventory)
            {
                state.Transition(PlayerUIStateType.Map);
                rightPanel.SwitchRight();
            }
        }
    }

    public void SwitchRight(InputAction.CallbackContext context)
    {
        if (context.canceled)
        {
            AudioManager.Play(AudioType.FlipOver);
            if (state.currentState is Map)
            {
                state.Transition(PlayerUIStateType.Inventory);
                rightPanel.SwitchLeft();
            }
            else if (state.currentState is Option)
            {
                state.Transition(PlayerUIStateType.Map);
                leftPanel.SwitchLeft();
            }
        }
    }

    public void QuitUI(InputAction.CallbackContext context)
    {
        if (context.canceled)
        {
            if (state.currentState is EnterUIState)
            {
                state.Transition(PlayerUIStateType.Normal);
            }
        }
    }

    #endregion

    public void OnClickLeft()
    {
        AudioManager.Play(AudioType.FlipOver);
        if (state.currentState is Map)
        {
            state.Transition(PlayerUIStateType.Option);
            leftPanel.SwitchRight();
        }
        else if (state.currentState is Option)
        {
            state.Transition(PlayerUIStateType.Map);
            leftPanel.SwitchLeft();
        }
    }

    public void OnClickRight()
    {
        AudioManager.Play(AudioType.FlipOver);
        if (state.currentState is Map)
        {
            state.Transition(PlayerUIStateType.Inventory);
            rightPanel.SwitchLeft();
        }
        else if (state.currentState is Inventory)
        {
            state.Transition(PlayerUIStateType.Map);
            rightPanel.SwitchRight();
        }
    }
}
