using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EnemyStates;

namespace LichStates
{
    public class RangeSpell : ReleaseAttackState
    {
        private LichController lich;

        public RangeSpell(LichController character) : base(character)
        {
            lich = character;
        }

        public override void EnterState()
        {
            base.EnterState();
            targetInfo.Update();
            anim.Play("RangeSpell");
            if (targetInfo.distance <= data.prepareAttackRadius - 2)
            {
                AudioManager.Play(AudioType.Dash, transform);
                character.rigid.AddForce(-targetInfo.direction * data.dashForce, ForceMode2D.Impulse);
            }
            lich.isRangeSpell = false;
        }

        public override void UpdateState()
        {
            AttackEnd("RangeSpell");
        }

        public override void ExitState()
        {

        }
    }

    public class LichChase : InCombatState
    {

        public LichChase(SkeletonArcherController character) : base(character)
        {

        }

        public override void EnterState()
        {
            targetInfo.Update();
            if (targetInfo.distance <= data.prepareAttackRadius - 1.2f)
            {
                StateTransition(CharacterStateType.AwaitAttack);
            }
            else
            {
                anim.Play("Move");
            }
        }

        public override void UpdateState()
        {
            targetInfo.Update();
            if (targetInfo.distance <= data.prepareAttackRadius - 1.2f)
            {
                StateTransition(CharacterStateType.AwaitAttack);
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

    public class LichKeepDistance : InCombatState
    {
        private Timer spellTimer;

        public LichKeepDistance(SkeletonArcherController character) : base(character)
        {
            spellTimer = character.spellTimer;
        }

        public override void EnterState()
        {
            anim.Play("Move");
        }

        public override void UpdateState()
        {
            targetInfo.Update();
            if (spellTimer.ReachedTime())
            {
                spellTimer.NextTime();
                StateTransition(CharacterStateType.Spell);
            }
            else if (targetInfo.distance <= data.prepareAttackRadius - 1.2f)
            {
                MoveTo(targetInfo.localPosition - targetInfo.direction * data.prepareAttackRadius);
                character.look = targetInfo.direction;
            }
            else if (targetInfo.distance <= data.viewRadius + GameManager.setting.fixedDistance)
            {
                StateTransition(CharacterStateType.Chase);
            }
            else base.UpdateState();
        }

        public override void ExitState()
        {
            character.move = Vector2.zero;
        }
    }

    public class LichAwaitAttack : InCombatState
    {
        private Timer attackTimer;
        private Timer spellTimer;

        public LichAwaitAttack(SkeletonArcherController character) : base(character)
        {
            attackTimer = new Timer(data.attackBlank);
            spellTimer = character.spellTimer;
        }

        public override void EnterState()
        {
            targetInfo.Update();
            if (targetInfo.distance <= data.prepareAttackRadius - 2f && attackTimer.ReachedTime())
            {
                attackTimer.NextTime();
                StateTransition(CharacterStateType.PrepareAttack);
            }
            else if (targetInfo.distance <= data.prepareAttackRadius)
            {
                if (spellTimer.ReachedTime())
                {
                    spellTimer.NextTime();
                    StateTransition(CharacterStateType.Spell);
                }
                else if (targetInfo.distance <= data.prepareAttackRadius - 2f)
                {
                    StateTransition(CharacterStateType.KeepDistance);
                }
                else anim.Play("Idle");
            }
            else anim.Play("Idle");
        }

        public override void UpdateState()
        {
            targetInfo.Update();
            if (targetInfo.distance <= data.attackRadius && attackTimer.ReachedTime())
            {
                attackTimer.NextTime();
                StateTransition(CharacterStateType.PrepareAttack);
            }
            else if (targetInfo.distance <= data.prepareAttackRadius)
            {
                if (spellTimer.ReachedTime())
                {
                    spellTimer.NextTime();
                    StateTransition(CharacterStateType.Spell);
                }
                else if (targetInfo.distance <= data.prepareAttackRadius - 2f)
                {
                    StateTransition(CharacterStateType.KeepDistance);
                }
            }
            else if (targetInfo.distance <= data.viewRadius)
            {
                StateTransition(CharacterStateType.Chase);
            }
            else base.UpdateState();
        }

        public override void ExitState()
        {

        }
    }

    public class LichAfterDeath : EnemyBaseState
    {

        public LichAfterDeath(EnemyController character) : base(character)
        {
            
        }

        public override void EnterState()
        {
            GameObject[] heelers = GameObject.FindGameObjectsWithTag("Enemy");
            foreach (GameObject heeler in heelers)
            {
                if (heeler != character.gameObject)
                {
                    GameObject.Destroy(heeler);
                }
            }
        }

        public override void UpdateState()
        {
            
        }

        public override void ExitState()
        {

        }
    }
}
