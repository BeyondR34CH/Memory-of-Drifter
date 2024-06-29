using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameEndStates
{

    public class Ready : IState
    {
        public Ready(BossTrigger trigger)
        {

        }

        public void EnterState()
        {
            
        }

        public void UpdateState()
        {
            
        }

        public void ExitState()
        {
            
        }
    }

    public class Start : IState
    {
        private BossTrigger trigger;

        private int num;
        private Timer timer;

        public Start(BossTrigger trigger)
        {
            this.trigger = trigger;
            num = 0;
            timer = new Timer(trigger.textKeepTime);
        }

        public void EnterState()
        {
            AudioManager.Play(AudioType.YouWin);
            AudioManager.instance.music.Stop();
            GameManager.playerinput.enabled = false;

            timer.NextTime();
            UIManager.instance.endText.text = trigger.texts[num++];
        }

        public void UpdateState()
        {
            if (timer.ReachedTime())
            {
                if (num >= trigger.texts.Length)
                {
                    trigger.state.Transition(GameEndType.End);
                }
                else if (num == 1)
                {
                    AudioManager.instance.music.clip = AudioManager.instance.endMusic;
                    AudioManager.instance.music.Play();
                }
                timer.NextTime();
                UIManager.instance.endText.text = trigger.texts[num++];
            }
        }

        public void ExitState()
        {

        }
    }

    public class End : IState
    {
        private BossTrigger trigger;

        public End(BossTrigger trigger)
        {
            this.trigger = trigger;
        }

        public void EnterState()
        {
            UIManager.instance.endText.text = "";
            GameManager.playerinput.enabled = true;
            trigger.transPoint.SetActive(true);
        }

        public void UpdateState()
        {

        }

        public void ExitState()
        {

        }
    }
}
