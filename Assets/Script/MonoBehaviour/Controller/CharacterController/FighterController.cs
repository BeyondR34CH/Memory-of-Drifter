using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class FighterController : CharacterController<FighterData>
{
    //event
    public event Action OnDash;
    public event Action OnAttack;
    public event Action OnDefence;
    
    protected internal FighterController target;

    protected override void LoadEffect()
    {
        base.LoadEffect();
        effect.Relate(EffectStateType.Defence, new EffectStates.Defence(this));
    }

    protected override void OnDrawGizmos()
    {
        if (showJudge)
        {
            base.OnDrawGizmos();
            //¹¥»÷Ô¤±¸·¶Î§
            Gizmos.color = Color.white;
            Gizmos.DrawWireSphere(transform.position, data.prepareAttackRadius);
            //¹¥»÷·¶Î§
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, data.attackRadius);

            Gizmos.DrawLine(transform.position, transform.position + Tools.RotateVector(look * data.attackRadius,  data.attackAngle));
            Gizmos.DrawLine(transform.position, transform.position + Tools.RotateVector(look * data.attackRadius, -data.attackAngle));
            //½ÇÉ«³¯Ïò
            Gizmos.color = Color.white;
            Gizmos.DrawLine(transform.position, transform.position + (Vector3)look * data.attackRadius);
        }
    }

    public override void EnterHurt(Transform target, int damage, bool interrupt, float repelForce)
    {
        base.EnterHurt(target, damage, interrupt, repelForce);
        this.target = target.GetComponent<FighterController>();
        if (interrupt)
        {
            state.Transition(CharacterStateType.Hurt);
            rigid.AddForce(-look * repelForce, ForceMode2D.Impulse);
        }
        else if (currentHealth <= 0)
        {
            state.Transition(CharacterStateType.Death);
        }
    }

    public void EnterDash() => OnDash?.Invoke();

    public void EnterAttack() => OnAttack?.Invoke();

    public void EnterDefence()
    {
        OnDefence?.Invoke();

        effect.Transition(EffectStateType.Defence);
    }
}
