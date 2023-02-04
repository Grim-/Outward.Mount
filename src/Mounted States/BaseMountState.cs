using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace EmoMount.Mounted_States
{
    public class BaseMountState : BaseState<BasicMountController>
    {
        public override void OnEnter(BasicMountController controller)
        {
           
        }

        public override void OnExit(BasicMountController controller)
        {
          
        }

        public override void OnFixedUpdate(BasicMountController controller)
        {
          
        }

        public override void OnUpdate(BasicMountController controller)
        {
            UpdateAnimator(controller);
            UpdatePlayerInput(controller);
        }

        public virtual void UpdateAnimator(BasicMountController MountController)
        {

        }

        public virtual void UpdatePlayerInput(BasicMountController controller)
        {
            if (controller.CharacterOwner)
            {
                if (ControlsInput.DodgeButtonDown(controller.CharacterOwner.OwnerPlayerSys.PlayerID))
                {
                    controller.EventComp.OnDodgeDown?.Invoke(controller.CharacterOwner);
                }

                if (ControlsInput.Sprint(controller.CharacterOwner.OwnerPlayerSys.PlayerID))
                {
                    controller.EventComp.OnSprintHeld?.Invoke(controller.CharacterOwner);
                }

                if (ControlsInput.Interact(controller.CharacterOwner.OwnerPlayerSys.PlayerID))
                {
                    controller.EventComp.OnInteract?.Invoke(controller.CharacterOwner);
                }
            }
        }
    }
}
