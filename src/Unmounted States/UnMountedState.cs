using SideLoader;
using UnityEngine;

namespace EmoMount
{
    //The Base Unmounted state
    public class UnMountedState : BaseUnMountedState
    {
        private Vector3 MoveToTarget = Vector3.zero;
        private bool StayStill = false;
        private bool ShowDismissInteraction = false;

        public override void OnEnter(BasicMountController MountController)
        {
            MountController.EnableNavMeshAgent();
       
            if (EmoMountMod.Debug)
            {
                MountController.DisplayNotification($"{MountController.MountName}, entered Leash State");
            }
           
        }

        public override void OnExit(BasicMountController MountController)
        {
            MoveToTarget = Vector3.zero;
            StayStill = false;
            if (EmoMountMod.Debug)
            {
                MountController.DisplayNotification($"{MountController.MountName}, left Leash State");
            }
        }

        public override void OnFixedUpdate(BasicMountController MountController)
        {

        }

        public override void OnUpdate(BasicMountController MountController)
        {
            base.OnUpdate(MountController);

            if (CustomKeybindings.GetKeyDown(EmoMountMod.MOUNT_DISMOUNT_KEY))
            {
                ShowDismissInteraction = !ShowDismissInteraction;


                if (ShowDismissInteraction)
                {
                    MountController.interactionActivator.AddBasicInteractionOverride(MountController.dismissMountInteraction);
                }
                else
                {
                    MountController.interactionActivator.RemoveBasicInteractionOverride(MountController.dismissMountInteraction);
                }

            }

            if (CustomKeybindings.GetKeyDown(EmoMountMod.MOUNT_FOLLOW_WAIT_TOGGLE))
            {
                StayStill = !StayStill;

                if (StayStill)
                {
                    MountController.DisplayNotification($"{MountController.MountName}, stay.");
                }
                else
                {
                    MountController.DisplayNotification($"{MountController.MountName}, to me.");
                }
            }


            if (!StayStill)
            {
                if (MoveToTarget != Vector3.zero)
                {
                    MoveToTargetPosition(MountController, MoveToTarget);
                }
                else
                {
                    if (MountController.DistanceToOwner > MountController.LeashDistance)
                    {
                        MoveToOwner(MountController);
                    }                  
                }
              
            }
            else
            {
                MountController.NavMesh.isStopped = true;
                MoveToTarget = Vector3.zero;
            }
    
            CheckMoveToInput(MountController);
        }


        private void MoveToOwner(BasicMountController MountController)
        {
            StayStill = true;

            Vector3 LeashPosition = MountController.CharacterOwner.transform.position + (Vector3)Random.insideUnitCircle * MountController.LeashPointRadius;

            if (LeashPosition != Vector3.zero)
            {
                MoveToTargetPosition(MountController, LeashPosition);
            }
        }

        private void MoveToTargetPosition(BasicMountController MountController, Vector3 MoveToTarget)
        {
            StayStill = false;

            if (MoveToTarget != Vector3.zero)
            {
                float distanceToTarget = Vector3.Distance(MountController.transform.position, MoveToTarget);

                if (distanceToTarget <= MountController.LeashDistance)
                {
                    MountController.NavMesh.isStopped = true;
                }
                else
                {

                    MountController.NavMesh.SetDestination(MoveToTarget);
                    MountController.NavMesh.isStopped = false;
                }
            }
        }
        private void CheckMoveToInput(BasicMountController MountController)
        {

            if (CustomKeybindings.GetKeyDown(EmoMountMod.MOUNT_MOVE_TO_KEY))
            {
                ///middle of screen into world space
                Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5F, 0.5F, 0));

                if (Physics.Raycast(ray, out RaycastHit hit, MountController.MoveToRayCastDistance, MountController.MoveToLayerMask))
                {
                    MoveToTarget = hit.point;
                    MountController.DisplayNotification($"{MountController.MountName}! Go!");
                }
            }
        }

    }
}
