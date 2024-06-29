using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EnemyStates;

namespace ArcherStates
{
    public class ArcherChase : InCombatState
    {

        public ArcherChase(SkeletonArcherController character) : base(character)
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

    public class ArcherKeepDistance : InCombatState
    {
        private Timer spellTimer;

        public ArcherKeepDistance(SkeletonArcherController character) : base(character)
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

    public class ArcherAwaitAttack : InCombatState
    {
        private Timer meleeTimer;
        private Timer spellTimer;

        public ArcherAwaitAttack(SkeletonArcherController character) : base(character)
        {
            meleeTimer = new Timer(data.attackBlank);
            spellTimer = character.spellTimer;
        }

        public override void EnterState()
        {
            targetInfo.Update();
            if (targetInfo.distance <= data.attackRadius && meleeTimer.ReachedTime())
            {
                meleeTimer.NextTime();
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
            if (targetInfo.distance <= data.attackRadius && meleeTimer.ReachedTime())
            {
                meleeTimer.NextTime();
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

    public class ArcherPrepareAttack : InCombatState
    {
        public ArcherPrepareAttack(EnemyController character) : base(character)
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
                StateTransition(CharacterStateType.MeleeAttack);
            }
        }

        public override void ExitState()
        {

        }
    }
}
