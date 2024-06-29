using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CommonStates
{
    public abstract class CommonState : CharacterState
    {
        protected CharacterController character;

        public CommonState(CharacterController character) : base(character)
        {
            this.character = character;
        }
    }

    public class Death : CommonState
    {
        private Collider2D collider;

        public Death(CharacterController character) : base(character)
        {
            collider = character.GetComponent<Collider2D>();
        }

        public override void EnterState()
        {
            anim.Play("Death");
            AudioManager.Play(AudioType.Death, transform);
            collider.enabled = false;
            character.move = Vector2.zero;
            character.EnterDeath();
        }

        public override void UpdateState()
        {

        }

        public override void ExitState()
        {

        }
    }
}
