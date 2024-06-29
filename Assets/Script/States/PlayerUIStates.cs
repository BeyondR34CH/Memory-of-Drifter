using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PlayerUIStates
{
    public class Normal : PlayerUIState
    {
        public Normal(PlayerUIController controller) : base(controller)
        {

        }

        public override void EnterState()
        {
            GameManager.playerinput.SwitchCurrentActionMap("Player");
            AudioManager.Play(AudioType.CloseView);
            canvas.sortingOrder = 0;
        }

        public override void UpdateState()
        {
            
        }

        public override void ExitState()
        {
            GameManager.playerinput.SwitchCurrentActionMap("UI");
            AudioManager.Play(AudioType.OpenView);
            canvas.sortingOrder = 1;
        }
    }

    public abstract class EnterUIState : PlayerUIState
    {
        protected ContentPanel panel;
        protected LRPanel left;
        protected LRPanel right;

        protected float LRSize;

        public EnterUIState(PlayerUIController controller) : base(controller)
        {
            left = controller.panels[1] as LRPanel;
            right = controller.panels[3] as LRPanel;

            LRSize = (controller.panels[1].openSize.x - 4) / 2 * rate;
        }

        public override void EnterState()
        {
            panel.Open();
        }

        public override void UpdateState()
        {

        }

        public override void ExitState()
        {
            panel.Close();
        }
    }

    public class Option : EnterUIState
    {
        private Vector2 optionSize;

        public Option(PlayerUIController controller) : base(controller)
        {
            panel = controller.panels[0] as ContentPanel;
            optionSize = new Vector2((controller.panels[0].openSize.x - 4) / 2 * rate, (controller.panels[0].openSize.y - 9) * rate);
        }

        public override void EnterState()
        {
            base.EnterState();

            InventoryManager.instance.eventSystem.SetSelectedGameObject(controller.optionEnter);

            left.Open(1);

            currentPositions[1].localPosition = new Vector3(defaultPositions[1].localPosition.x - LRSize, 0);
            currentPositions[0].localPosition = new Vector3(currentPositions[1].localPosition.x * 2 - optionSize.x, optionSize.y);

            panelFollow[1].target = currentPositions[1];
            panelFollow[0].target = currentPositions[0];
        }

        public override void ExitState()
        {
            PlayerPrefs.Save();

            base.ExitState();
            left.Close();

            panelFollow[1].target = defaultPositions[1];
            panelFollow[0].target = defaultPositions[0];
        }
    }

    public class Map : EnterUIState
    {
        private Vector2 mapSize;

        public Map(PlayerUIController controller) : base(controller)
        {
            panel = controller.panels[2] as ContentPanel;
            mapSize = new Vector2((controller.panels[2].openSize.x - 4) / 2 * rate, (controller.panels[2].openSize.y) * rate);
        }

        public override void EnterState()
        {
            base.EnterState();

            InventoryManager.instance.eventSystem.SetSelectedGameObject(null);

            left.Open(-1);
            right.Open(1);

            currentPositions[2].localPosition = defaultPositions[2].localPosition;

            currentPositions[1].localPosition = new Vector3(defaultPositions[1].localPosition.x - mapSize.x - LRSize, 0);
            currentPositions[3].localPosition = new Vector3(defaultPositions[3].localPosition.x + mapSize.x + LRSize, 0);

            currentPositions[0].localPosition = new Vector3(currentPositions[1].localPosition.x - LRSize - 5 * rate, 0);
            currentPositions[4].localPosition = new Vector3(currentPositions[3].localPosition.x + LRSize + 5 * rate, 0);

            for (int i = 0; i < controller.cellCount; i++)
            {
                panelFollow[i].target = currentPositions[i];
            }
        }

        public override void ExitState()
        {
            base.ExitState();
            left.Close();
            right.Close();

            for (int i = 0; i < controller.cellCount; i++)
            {
                panelFollow[i].target = defaultPositions[i];
            }
        }
    }

    public class Inventory : EnterUIState
    {
        private Vector2 inventorySize;

        public Inventory(PlayerUIController controller) : base(controller)
        {
            panel = controller.panels[4] as ContentPanel;
            inventorySize = new Vector2((controller.panels[4].openSize.x - 4) / 2 * rate, (controller.panels[4].openSize.y - 9) * rate);
        }

        public override void EnterState()
        {
            base.EnterState();

            InventoryManager.instance.eventSystem.SetSelectedGameObject(controller.inventoryEnter);

            right.Open(-1);

            currentPositions[3].localPosition = new Vector3(defaultPositions[3].localPosition.x + LRSize, 0);
            currentPositions[4].localPosition = new Vector3(currentPositions[3].localPosition.x * 2 + inventorySize.x, inventorySize.y);

            panelFollow[3].target = currentPositions[3];
            panelFollow[4].target = currentPositions[4];
        }

        public override void ExitState()
        {
            InventoryManager.instance.playerUI.currentGrid = null;

            base.ExitState();
            right.Close();

            panelFollow[3].target = defaultPositions[3];
            panelFollow[4].target = defaultPositions[4];
        }
    }
}
