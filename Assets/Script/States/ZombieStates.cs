using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EnemyStates;

namespace ZombieStates
{
    public class ZombieAwaitBorn : OutCombatState
    {
        public ZombieAwaitBorn(ZombieController character) : base(character)
        {

        }

        public override void EnterState()
        {
            
        }

        public override void UpdateState()
        {
            if (SearchPlayer())
            {
                StateTransition(CharacterStateType.Born);
            }
        }

        public override void ExitState()
        {

        }
    }

    public class ZombiePrepareAttack : InCombatState
    {
        private bool bite;
        private bool canBite;

        public ZombiePrepareAttack(ZombieController character) : base(character)
        {
            canBite = character.canBite;
        }

        public override void EnterState()
        {
            character.releaseAttack = false;
            targetInfo.Update();
            character.look = targetInfo.direction;
            if (canBite && targetInfo.distance <= data.attackRadius)
            {
                bite = true;
                anim.Play("Bite");
            }
            else
            {
                bite = false;
                anim.Play("Attack");
            }
        }

        public override void UpdateState()
        {
            targetInfo.Update();
            character.look = targetInfo.direction;
            if (character.releaseAttack)
            {
                if (bite)
                {
                    StateTransition(CharacterStateType.MeleeAttack);
                }
                else
                {
                    StateTransition(CharacterStateType.SpecialAttack);
                }
            }
        }

        public override void ExitState()
        {

        }
    }

    public class ZombieBiteAttack : ReleaseAttackState
    {

        public ZombieBiteAttack(EnemyController character) : base(character)
        {

        }

        public override void EnterState()
        {
            base.EnterState();
            AudioManager.Play(AudioType.BiteAttack, character.transform);
            character.EnterAttack();
            character.rigid.AddForce(targetInfo.direction * data.attackStep, ForceMode2D.Impulse);
        }

        public override void UpdateState()
        {
            Collider2D target = Physics2D.OverlapCircle(transform.position, data.attackRadius, LayerMask.GetMask("Player"));
            if (!hasHit && target &&
                CanSee(target.transform.position - transform.position) &&
                Tools.Angle(character.look, (target.transform.position - transform.position).normalized) <= data.attackAngle)
            {
                target.GetComponent<CharacterController>().EnterHurt(transform, data.attackDamage, true, data.repelForce);
                AudioManager.Play(AudioType.BiteHit, target.transform);
                hasHit = true;
            }
            AttackEnd("Bite");
        }

        public override void ExitState()
        {

        }
    }

    public class ZombieDeath : OutCombatState
    {
        private Timer timer;
        private Collider2D collider;
        private NPCUIController ui;

        public ZombieDeath(ZombieController character) : base(character)
        {
            timer = new Timer(character.rebornTime);
            collider = character.GetComponent<Collider2D>();
            ui = character.GetComponent<NPCUIController>();
        }

        public override void EnterState()
        {
            timer.NextTime();
            anim.Play("Death");
            AudioManager.Play(AudioType.Death, transform);
            collider.enabled = false;
            character.move = Vector2.zero;
            character.EnterDeath();
            ui.enabled = false;
        }

        public override void UpdateState()
        {
            if (timer.ReachedTime() && SearchPlayer())
            {
                StateTransition(CharacterStateType.Reborn);
            }
        }

        public override void ExitState()
        {

        }
    }

    public class ZombieBorn : InCombatState
    {
        private Collider2D collider;
        private NPCUIController ui;

        public ZombieBorn(ZombieController character) : base(character)
        {
            collider = character.GetComponent<Collider2D>();
            ui = character.GetComponent<NPCUIController>();
        }

        public override void EnterState()
        {
            anim.Play("Born");
            character.currentHealth = character.maxHealth;
        }

        public override void UpdateState()
        {
            if (AnimPlayDone("Born", 0.95f))
            {
                StateTransition(CharacterStateType.Idle);
            }
        }

        public override void ExitState()
        {
            collider.enabled = true;
            ui.enabled = true;
        }
    }

    public class ZombieReborn : InCombatState
    {
        private Collider2D collider;
        private NPCUIController ui;

        public ZombieReborn(ZombieController character) : base(character)
        {
            collider = character.GetComponent<Collider2D>();
            ui = character.GetComponent<NPCUIController>();
        }

        public override void EnterState()
        {
            anim.Play("Reborn");
            character.currentHealth = character.maxHealth;
        }

        public override void UpdateState()
        {
            if (AnimPlayDone("Reborn", 0.95f))
            {
                StateTransition(CharacterStateType.Idle);
            }
        }

        public override void ExitState()
        {
            collider.enabled = true;
            ui.enabled = true;
        }
    }
}
