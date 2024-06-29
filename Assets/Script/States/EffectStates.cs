using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EffectStates
{
    public class Normal : EffectState
    {
        public Normal(CharacterController character) : base(character)
        {

        }

        public override void EnterState()
        {
            
        }

        public override void UpdateState()
        {
            
        }

        public override void ExitState()
        {
            
        }
    }

    #region PauseFrameState

    public abstract class PauseFrameState : EffectState
    {
        protected abstract Color color { get; }

        protected Timer timer;

        public PauseFrameState(CharacterController character) : base(character)
        {
            timer = new Timer(GameManager.setting.pauseFrameTime);
        }

        public override void EnterState()
        {
            timer.NextTime();
            anim.speed = GameManager.setting.pauseFrameRate;
            sprite.material.SetColor("_Color", color);
            sprite.material.SetFloat("_Alpha", 1);
        }

        public override void UpdateState()
        {
            if (timer.ReachedTime())
            {
                StateTransition(EffectStateType.Normal);
            }
        }

        public override void ExitState()
        {
            anim.speed = 1;
            sprite.material.SetFloat("_Alpha", 0);
        }
    }

    public class Hurt : PauseFrameState
    {
        protected override Color color => Color.white;

        public Hurt(CharacterController character) : base(character)
        {

        }
    }

    public class Defence : PauseFrameState
    {
        protected override Color color => Color.yellow;

        public Defence(CharacterController character) : base(character)
        {

        }
    }

    #endregion

    #region GradientFeedbackState

    public abstract class GradientFeedbackState : EffectState
    {
        protected abstract Color color { get; }

        protected float currentAlpha;

        public GradientFeedbackState(CharacterController character) : base(character)
        {
            
        }

        public override void EnterState()
        {
            currentAlpha = 1;
            sprite.material.SetColor("_Color", color);
            sprite.material.SetFloat("_Alpha", 1);
        }

        public override void UpdateState()
        {
            if (currentAlpha >= 0.05f)
            {
                currentAlpha = Mathf.SmoothStep(currentAlpha, 0, GameManager.setting.colorFadeSpeed);
                sprite.material.SetFloat("_Alpha", currentAlpha);
            }
            else
            {
                StateTransition(EffectStateType.Normal);
            }
        }

        public override void ExitState()
        {
            sprite.material.SetFloat("_Alpha", 0);
        }
    }

    public class Heal : GradientFeedbackState
    {
        protected override Color color => Color.cyan;

        public Heal(CharacterController character) : base(character)
        {

        }

        public override void EnterState()
        {
            base.EnterState();
            AudioManager.Play(AudioType.Heal, character.transform);
        }
    }

    #endregion

    public class Dash : EffectState
    {
        private GameObject shadowPrefab;
        private int shadowCount;
        private Color color_1;
        private Color color_2;

        private bool switchColor;
        private int num;
        private Timer timer;

        public Dash(CharacterController character) : base(character)
        {
            shadowPrefab = Resources.Load<GameObject>("Prefab/Other/DashShadow");
            shadowCount = GameManager.setting.shadowCount;
            color_1 = GameManager.setting.DashShadowColor_1;
            color_2 = GameManager.setting.DashShadowColor_2;

            timer = new Timer(GameManager.setting.shadowBlank);
        }

        public override void EnterState()
        {
            num = 0;
        }

        public override void UpdateState()
        {
            if (timer.ReachedTime())
            {
                if (num++ < shadowCount)
                {
                    timer.NextTime();
                    DashShadow shadow = EffectPool.GetObject(shadowPrefab).GetComponent<DashShadow>();
                    switchColor = !switchColor;
                    shadow.SetShadow(character.transform, switchColor ? color_1 : color_2);
                }
                else
                {
                    StateTransition(EffectStateType.Normal);
                }
            }
        }

        public override void ExitState()
        {
            
        }
    }
}
