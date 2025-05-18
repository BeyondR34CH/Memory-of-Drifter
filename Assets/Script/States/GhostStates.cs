using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EnemyStates;

namespace GhostStates
{
    public abstract class OutCombatState : EnemyBaseState
    {
        public OutCombatState(EnemyController character) : base(character)
        {

        }

        protected bool SearchPlayer()
        {
            Collider2D player = Physics2D.OverlapCircle(transform.position, data.viewRadius, LayerMask.GetMask("Player"));
            if (player)
            {
                character.target = player.GetComponent<FighterController>();
                return true;
            }
            else return false;
        }
    }

    public class NonObstacleIdle : OutCombatState
    {
        private Timer timer;

        public NonObstacleIdle(EnemyController character) : base(character)
        {
            timer = new Timer(character.patrolWaitTime);
        }

        public override void EnterState()
        {
            anim.Play("Idle");
            timer.NextTime(Random.Range(0, 1));
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

    public class NonObstaclePatrol : OutCombatState
    {
        private int current;
        private Vector3 localPoint;

        public NonObstaclePatrol(GhostController character) : base(character)
        {
            current = 0;
        }

        public override void EnterState()
        {
            if (character.canPatrol)
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
                else
                {
                    character.move = localPoint.normalized;
                    character.look = localPoint.normalized;
                }
            }
        }

        public override void ExitState()
        {
            character.move = Vector2.zero;
        }
    }

    public class NonObstacleChase : InCombatState
    {

        public NonObstacleChase(EnemyController character) : base(character)
        {
            
        }

        public override void EnterState()
        {
            targetInfo.Update();
            if (targetInfo.distance <= data.prepareAttackRadius)
            {
                StateTransition(CharacterStateType.AwaitAttack);
            }
            else anim.Play("Move");
        }

        public override void UpdateState()
        {
            targetInfo.Update();
            if (targetInfo.distance <= data.prepareAttackRadius)
            {
                StateTransition(CharacterStateType.AwaitAttack);
            }
            else if (targetInfo.distance <= data.viewRadius + GameManager.setting.fixedDistance)
            {
                character.move = targetInfo.direction;
                character.look = targetInfo.direction;
            }
            else base.UpdateState();
        }

        public override void ExitState()
        {
            character.move = Vector2.zero;
        }
    }
}
