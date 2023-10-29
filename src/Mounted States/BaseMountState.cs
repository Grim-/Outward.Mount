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

        public virtual void OnSprint(BasicMountController controller)
        {

        }

        public virtual void OnDodge(BasicMountController controller)
        {

        }

        public virtual void OnInteract(BasicMountController controller)
        {

        }

        public virtual void OnAttack1(BasicMountController controller)
        {

        }

        public virtual void OnBlockHeld(BasicMountController controller)
        {

        }

        public virtual void OnBlockReleased(BasicMountController controller)
        {

        }

        public virtual void OnCrouch(BasicMountController controller)
        {

        }

        public virtual void OnAttack2(BasicMountController controller)
        {

        }

        public virtual void UpdateAnimator(BasicMountController MountController)
        {
            if (MountController.NavMeshAgent.enabled)
            {
                float forwardVel = Vector3.Dot(MountController.NavMeshAgent.velocity.normalized, MountController.transform.forward);
                float sideVel = Vector3.Dot(MountController.NavMeshAgent.velocity.normalized, MountController.transform.right);

                MountController.IsMoving = MountController.NavMeshAgent.velocity != Vector3.zero;
                //dont even use X its just there because 
                MountController.Animator.SetFloat("Move X", sideVel, 5f, 5f);
                MountController.Animator.SetFloat("Move Z", forwardVel, 5f, 5f);
            }
            else
            {
                MountController.IsMoving = MountController.BaseInput.z != 0;

                MountController.Animator.SetFloat("Move X", MountController.BaseInput.x, 5f, 5f);

                float TargetZ = 0;

                if (MountController.BaseInput.z != 0 || MountController.BaseInput.x != 0)
                {
                    TargetZ = MountController.IsSprinting ? 1f : 0.5f;
                }

                MountController.Animator.SetFloat("Move Z", TargetZ, 0.5f, Time.deltaTime * 2f);
            }
        }

        public virtual void UpdatePlayerInput(BasicMountController controller)
        {
            if (controller.CharacterOwner)
            {
                if (ControlsInput.DodgeButtonDown(controller.CharacterOwner.OwnerPlayerSys.PlayerID))
                {
                    OnDodge(controller);
                    controller.EventComp.OnDodgeDown?.Invoke(controller, controller.CharacterOwner);
                }

                if (ControlsInput.Sprint(controller.CharacterOwner.OwnerPlayerSys.PlayerID))
                {
                    OnSprint(controller);
                    controller.EventComp.OnSprintHeld?.Invoke(controller, controller.CharacterOwner);
                }

                if (ControlsInput.Interact(controller.CharacterOwner.OwnerPlayerSys.PlayerID))
                {
                    OnInteract(controller);
                    controller.EventComp.OnInteract?.Invoke(controller, controller.CharacterOwner);
                }

                if (ControlsInput.Block(controller.CharacterOwner.OwnerPlayerSys.PlayerID))
                {
                    OnBlockHeld(controller);
                    controller.EventComp.OnRightClickHeld?.Invoke(controller, controller.CharacterOwner);
                }
                else if(ControlsInput.BlockRelease(controller.CharacterOwner.OwnerPlayerSys.PlayerID))
                {
                    OnBlockReleased(controller);
                    controller.EventComp.OnRightClick?.Invoke(controller, controller.CharacterOwner);
                }

                if (ControlsInput.StealthButton(controller.CharacterOwner.OwnerPlayerSys.PlayerID))
                {
                    OnCrouch(controller);
                    controller.EventComp.OnCrouch?.Invoke(controller, controller.CharacterOwner);
                }

                if (ControlsInput.Attack1Release(controller.CharacterOwner.OwnerPlayerSys.PlayerID))
                {
                    OnAttack1(controller);
                    controller.EventComp.OnLeftClick?.Invoke(controller, controller.CharacterOwner);
                }

                if (ControlsInput.Attack2Release(controller.CharacterOwner.OwnerPlayerSys.PlayerID))
                {
                    OnAttack2(controller);
                    controller.EventComp.OnShiftLeftClick?.Invoke(controller, controller.CharacterOwner);
                }
            }
        }
    }
}
