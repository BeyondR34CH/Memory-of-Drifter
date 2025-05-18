using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PlayerStates
{
    #region UnfrozenState

    public abstract class UnfrozenState : PlayerBaseState
    {
        public UnfrozenState(PlayerController character) : base(character)
        {

        }

        public override void UpdateState()
        {
            character.move = character.moveInput;
            if (GameManager.setting.PlayerKeepFollowLook)
            {
                character.look = GetLookInput();
            }
            else if (!character.moveInput.Equals(Vector2.zero))
            {
                character.look = character.moveInput.normalized;
            }
        }
    }

    public class Idle : UnfrozenState
    {
        public Idle(PlayerController character) : base(character)
        {

        }

        public override void EnterState()
        {
            if (!character.moveInput.Equals(Vector2.zero))
            {
                StateTransition(CharacterStateType.Move);
            }
            else anim.Play("Idle");
        }

        public override void UpdateState()
        {
            base.UpdateState();
            if (!character.move.Equals(Vector2.zero))
            {
                StateTransition(CharacterStateType.Move);
            }
        }

        public override void ExitState()
        {

        }
    }

    public class Move : UnfrozenState
    {
        public Move(PlayerController character) : base(character)
        {

        }

        public override void EnterState()
        {
            anim.Play("Move");
        }

        public override void UpdateState()
        {
            base.UpdateState();
            if (character.move.Equals(Vector2.zero))
            {
                StateTransition(CharacterStateType.Idle);
            }
        }

        public override void ExitState()
        {
            character.move = Vector2.zero;
        }
    }

    public class Dash : UnfrozenState
    {
        public Dash(PlayerController character) : base(character)
        {
            
        }

        public override void EnterState()
        {
            if (!character.moveInput.Equals(Vector2.zero)) character.look = character.moveInput.normalized;
            anim.Play("Dash");
            AudioManager.Play(AudioType.Dash, character.transform);
            character.EnterDash();
            character.rigid.AddForce(character.look * data.dashForce, ForceMode2D.Impulse);
        }

        public override void UpdateState()
        {
            character.move = character.moveInput;
            if (AnimPlayDone("Dash", 0.75f))
            {
                StateTransition(CharacterStateType.Move);
            }
        }

        public override void ExitState()
        {

        }
    }

    #endregion

    #region FrozenState

    public abstract class FrozenState : PlayerBaseState
    {
        protected Timer timer;

        public FrozenState(PlayerController character) : base(character)
        {
                
        }

        public override void EnterState()
        {
            if (!character.lookInput.Equals(Vector2.zero))
            {
                character.look = GetLookInput();
            }
            else if (!character.moveInput.Equals(Vector2.zero))
            {
                character.look = character.moveInput.normalized;
            }
            character.move = Vector2.zero;
            character.rigid.velocity = Vector2.zero;
        }
    }

    public class Attack : FrozenState
    {
        private bool switchAttack;
        private Collider2D[] targets;
        private Dictionary<Collider2D, bool> hasHit;

        public Attack(PlayerController character) : base(character)
        {
            targets = new Collider2D[10];
            hasHit = new Dictionary<Collider2D, bool>();
        }

        public override void EnterState()
        {
            switchAttack = !switchAttack;
            anim.Play(switchAttack ? "Attack_1" : "Attack_2");
            AudioManager.Play(AudioType.Attack, character.transform);
            hasHit.Clear();
            character.EnterAttack();
            base.EnterState();
            character.rigid.AddForce(character.look * data.attackStep, ForceMode2D.Impulse);
        }

        public override void UpdateState()
        {
            int targetsCount = Physics2D.OverlapCircleNonAlloc(transform.position, data.attackRadius, targets, LayerMask.GetMask("Enemy"));
            for (int i = 0; i < targetsCount; i++)
            {
                if (!hasHit.ContainsKey(targets[i]) && 
                    CanSee(targets[i].transform.position - transform.position) &&
                    Tools.Angle(character.look, (targets[i].transform.position - transform.position).normalized) <= data.attackAngle)
                {
                    targets[i].GetComponent<CharacterController>().EnterHurt(transform, data.attackDamage, true, data.repelForce);
                    GameManager.CameraImpulse();
                    AudioManager.Play(AudioType.Hit, targets[i].transform);
                    hasHit.Add(targets[i], true);
                }
            }
            if (AnimPlayDone(switchAttack ? "Attack_1" : "Attack_2", 0.72f))
            {
                StateTransition(CharacterStateType.Idle);
            }
        }

        public override void ExitState()
        {

        }
    }

    public class Defence : FrozenState
    {
        public Defence(PlayerController character) : base(character)
        {
                
        }

        public override void EnterState()
        {
            anim.Play("Defence");
            base.EnterState();
            character.rigid.AddForce(character.look * data.defenceStep, ForceMode2D.Impulse);
        }

        public override void UpdateState()
        {
            if (AnimPlayDone("Defence", 1f))
            {
                StateTransition(CharacterStateType.Idle);
            }
        }

        public override void ExitState()
        {

        }
    }

    public class Hurt : FrozenState
    {
        public Hurt(PlayerController character) : base(character)
        {

        }

        public override void EnterState()
        {
            character.look = (character.target.transform.position - transform.position).normalized;
            character.rigid.velocity = Vector2.zero;
            if (character.currentHealth <= 0)
            {
                StateTransition(CharacterStateType.Death);
            }
            else
            {
                anim.Play("Hurt");
            }
        }

        public override void UpdateState()
        {
            if (!character.rigid.velocity.Equals(Vector2.zero)) character.look = -character.rigid.velocity.normalized;
            if (AnimPlayDone("Hurt", 0.72f))
            {
                StateTransition(CharacterStateType.Idle);
            }
        }

        public override void ExitState()
        {

        }
    }

    public class Heal : FrozenState
    {
        public Heal(PlayerController character) : base(character)
        {
                
        }

        public override void EnterState()
        {
            anim.Play("Heal");

            base.EnterState();
        }

        public override void UpdateState()
        {
            if (AnimPlayDone("Heal", 0.5f))
            {
                //Î´Íê³É
                character.EnterHeal();
                StateTransition(CharacterStateType.Idle);
            }
        }

        public override void ExitState()
        {

        }
    }

    #endregion
}
