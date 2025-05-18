using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EnemyStates
{
    #region OutCombatState

    public abstract class OutCombatState : EnemyBaseState
    {
        public OutCombatState(EnemyController character) : base(character)
        {

        }

        protected bool SearchPlayer()
        {
            Collider2D player = Physics2D.OverlapCircle(transform.position, data.viewRadius, LayerMask.GetMask("Player"));
            if (player && CanSee(player.transform.position - transform.position))
            {
                character.target = player.GetComponent<FighterController>();
                if (character.canCellPartner)
                {
                    Collider2D[] partners = new Collider2D[10];
                    int partnersCount = Physics2D.OverlapCircleNonAlloc(transform.position, data.viewRadius, partners, LayerMask.GetMask("Enemy"));
                    for (int i = 0; i < partnersCount; i++)
                    {
                        EnemyController partner = partners[i].GetComponent<EnemyController>();
                        if (partner.state.currentState is OutCombatState)
                        {
                            partner.target = character.target;
                            partner.state.Transition(CharacterStateType.Chase);
                        }
                    }
                }
                return true;
            }
            else return false;
        }
    }

    public class Idle : OutCombatState
    {
        private Timer timer;

        public Idle(EnemyController character) : base(character)
        {
            timer = new Timer(character.patrolWaitTime);
        }

        public override void EnterState()
        {
            anim.Play("Idle");
            timer.NextTime(UnityEngine.Random.Range(0, 1));
        }

        public override void UpdateState()
        {
            if (SearchPlayer())
            {
                StateTransition(CharacterStateType.Chase);
            }
            else if (character.canPatrol && timer.ReachedTime())
            {
                StateTransition(CharacterStateType.Patrol);
            }
        }

        public override void ExitState()
        {

        }
    }

    //OutCombat入口
    public class Patrol : OutCombatState
    {
        private int current;
        private bool canTo;
        private Vector3 localPoint;

        public Patrol(EnemyController character) : base(character)
        {
            current = 0;
            canTo = true;
        }

        public override void EnterState()
        {
            if (character.canPatrol && canTo)
            {
                if (++current == character.patrolPoints.Length) current = 0;
                anim.Play("Move");
            }
            else if (!character.canPatrol && (character.postPoints.position - transform.position).magnitude > 0.25f)
            {
                anim.Play("Move");
            }
            else StateTransition(CharacterStateType.Idle);
        }

        public override void UpdateState()
        {
            if (SearchPlayer())
            {
                StateTransition(CharacterStateType.Chase);
            }
            else
            {
                localPoint = character.canPatrol ? character.patrolPoints[current].position - transform.position
                                                 : character.postPoints.position - transform.position;
                if (localPoint.magnitude <= 0.25f)
                {
                    StateTransition(CharacterStateType.Idle);
                }
                else if (!MoveTo(localPoint))
                {
                    canTo = false;
                    Debug.Log("无法到达下个巡逻点");
                    StateTransition(CharacterStateType.Idle);
                }
            }
        }

        public override void ExitState()
        {
            character.move = Vector2.zero;
        }
    }

    #endregion

    #region IncombatState

    public abstract class InCombatState : EnemyBaseState
    {
        protected TargetInfo targetInfo;

        public InCombatState(EnemyController character) : base(character)
        {
            targetInfo = character.targetInfo;
        }

        public override void UpdateState()
        {
            if (targetInfo.distance > data.viewRadius + GameManager.setting.fixedDistance || character.target.currentHealth <= 0)
            {
                character.target = null;
                StateTransition(CharacterStateType.Patrol);
            }
        }

        protected void AttackEnd(string attackName)
        {
            if (AnimPlayDone(attackName, 0.95f))
            {
                if (character.target.currentHealth <= 0)
                {
                    character.target = null;
                    StateTransition(CharacterStateType.Patrol);
                }
                else StateTransition(CharacterStateType.Chase);
            }
        }
    }

    //InCombat入口
    public class Chase : InCombatState
    {
        private Timer timer;

        public Chase(EnemyController character) : base(character)
        {
            timer = new Timer(character.data.spellBlank);
        }

        public override void EnterState()
        {
            targetInfo.Update();
            if (targetInfo.distance <= data.prepareAttackRadius)
            {
                StateTransition(CharacterStateType.AwaitAttack);
            }
            else
            {
                anim.Play("Move");
                timer.NextTime();
            }
        }

        public override void UpdateState()
        {
            targetInfo.Update();
            if (targetInfo.distance <= data.prepareAttackRadius)
            {
                StateTransition(CharacterStateType.AwaitAttack);
            }
            else if (character.canSpell && timer.ReachedTime() && targetInfo.distance <= data.viewRadius)
            {
                StateTransition(CharacterStateType.Spell);
            }
            else if (targetInfo.distance <= data.viewRadius + GameManager.setting.fixedDistance)
            {
                if (!MoveTo(targetInfo.localPosition))
                {
                    character.target = null;
                    StateTransition(CharacterStateType.Patrol);
                }
            }
            else base.UpdateState();
        }

        public override void ExitState()
        {
            character.move = Vector2.zero;
        }
    }

    //Attack入口
    public class AwaitAttack : InCombatState
    {
        private Timer timer;

        public AwaitAttack(EnemyController character) : base(character)
        {
            timer = new Timer(data.attackBlank);
        }

        public override void EnterState()
        {
            if (timer.ReachedTime())
            {
                timer.NextTime();
                StateTransition(CharacterStateType.PrepareAttack);
            }
            else anim.Play("Idle");
        }

        public override void UpdateState()
        {
            targetInfo.Update();
            if (timer.ReachedTime())
            {
                timer.NextTime();
                StateTransition(CharacterStateType.PrepareAttack);
            }
            else if (targetInfo.distance > data.prepareAttackRadius)
            {
                StateTransition(CharacterStateType.Chase);
            }
            else base.UpdateState();
        }

        public override void ExitState()
        {

        }
    }

    public class PrepareAttack : InCombatState
    {
        public PrepareAttack(EnemyController character) : base(character)
        {

        }

        public override void EnterState()
        {
            anim.Play("Attack");

            character.look = targetInfo.direction;
            character.releaseAttack = false;
        }

        public override void UpdateState()
        {
            if (character.releaseAttack)
            {
                if (character.canDashAttack && targetInfo.distance > data.attackRadius)
                {
                    StateTransition(CharacterStateType.SpecialAttack);
                }
                else
                {
                    StateTransition(CharacterStateType.MeleeAttack);
                }
            }
        }

        public override void ExitState()
        {

        }
    }

    public abstract class ReleaseAttackState : InCombatState
    {
        protected bool hasHit;

        public ReleaseAttackState(EnemyController character) : base(character)
        {

        }

        public override void EnterState()
        {
            hasHit = false;
        }

        protected void SingleAttack()
        {
            Collider2D target = Physics2D.OverlapCircle(transform.position, data.attackRadius, LayerMask.GetMask("Player"));
            if (!hasHit && target && 
                CanSee(target.transform.position - transform.position) && 
                Tools.Angle(character.look, (target.transform.position - transform.position).normalized) <= data.attackAngle)
            {
                target.GetComponent<CharacterController>().EnterHurt(transform, data.attackDamage, true, data.repelForce);
                AudioManager.Play(AudioType.Hit, target.transform);
                hasHit = true;
            }
        }
    }

    public class MeleeAttack : ReleaseAttackState
    {

        public MeleeAttack(EnemyController character) : base(character)
        {

        }

        public override void EnterState()
        {
            base.EnterState();
            AudioManager.Play(AudioType.Attack, character.transform);
            character.EnterAttack();
            character.rigid.AddForce(targetInfo.direction * data.attackStep, ForceMode2D.Impulse);
        }

        public override void UpdateState()
        {
            SingleAttack();
            AttackEnd("Attack");
        }

        public override void ExitState()
        {

        }
    }

    public class DashAttack : ReleaseAttackState
    {

        public DashAttack(EnemyController character) : base(character)
        {
            
        }

        public override void EnterState()
        {
            base.EnterState();
            AudioManager.Play(AudioType.Attack, character.transform);
            AudioManager.Play(AudioType.Dash, character.transform);
            character.EnterAttack();
            character.rigid.AddForce(targetInfo.direction * data.dashForce, ForceMode2D.Impulse);
        }

        public override void UpdateState()
        {
            SingleAttack();
            AttackEnd("Attack");
        }

        public override void ExitState()
        {
            
        }
    }

    public class Spell : InCombatState
    {
        public Spell(EnemyController character) : base(character)
        {

        }

        public override void EnterState()
        {
            targetInfo.Update();
            character.look = targetInfo.direction;
            anim.Play("Spell");
            character.isSpell = false;
        }

        public override void UpdateState()
        {
            AttackEnd("Spell");
        }

        public override void ExitState()
        {

        }
    }

    #endregion

    #region OtherState

    public class Hurt : EnemyBaseState
    {
        public Hurt(EnemyController character) : base(character)
        {

        }

        public override void EnterState()
        {
            anim.Play("Hurt");

            character.look = (character.target.transform.position - transform.position).normalized;
            character.rigid.velocity = Vector2.zero;
            character.move = Vector2.zero;
        }

        public override void UpdateState()
        {
            if (!character.rigid.velocity.Equals(Vector2.zero)) character.look = -character.rigid.velocity.normalized;
            if (AnimPlayDone("Hurt", 0.95f))
            {
                if (character.currentHealth > 0)
                {
                    StateTransition(CharacterStateType.Chase);
                }
                else
                {
                    StateTransition(CharacterStateType.Death);
                }
            }
        }

        public override void ExitState()
        {

        }
    }

    public class AfterDeath : EnemyBaseState
    {
        private Timer timer;
        private GameObject item;

        public AfterDeath(EnemyController character) : base(character)
        {
            timer = new Timer(GameManager.setting.destroyTime);
            item = Resources.Load<GameObject>("Prefab/MapProps/Item");
        }

        public override void EnterState()
        {
            timer.NextTime();
        }

        public override void UpdateState()
        {
            if (timer.ReachedTime())
            {
                if (character.itemId >= 0) GameObject.Instantiate(item).GetComponent<ItemController>().SetItem(character.itemId, character.transform.position);
                GameObject.Destroy(character.postPoints.gameObject);
                GameObject.Destroy(character.gameObject);
            }
        }

        public override void ExitState()
        {

        }
    }

    #endregion
}
