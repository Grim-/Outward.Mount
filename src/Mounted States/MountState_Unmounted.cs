using EmoMount.Mounted_States;
using SideLoader;
using UnityEngine;

namespace EmoMount
{
    public class MountState_Unmounted : BaseMountState
    {
        private Vector3 MoveToTarget = Vector3.zero;
        private bool StayStill = false;

        public override void OnEnter(BasicMountController MountController)
        {
            MountController.EnableNavMeshAgent();     
            MountController.IsMounted = false;
        }

        public override void OnExit(BasicMountController MountController)
        {
            MoveToTarget = Vector3.zero;
            StayStill = false;
        }

        public override void OnFixedUpdate(BasicMountController MountController)
        {

        }

        public override void OnUpdate(BasicMountController MountController)
        {
            base.OnUpdate(MountController);

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
            Vector3 LeashPosition = MountController.CharacterOwner.transform.position + (Vector3)Random.insideUnitCircle * MountController.LeashPointRadius;

            if (LeashPosition != Vector3.zero)
            {
                MoveToTargetPosition(MountController, LeashPosition);
            }
        }
        private void MoveToTargetPosition(BasicMountController MountController, Vector3 MoveToTarget)
        {
            if (!MountController.NavMesh.isOnNavMesh)
            {
                return;
            }

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

        public override void UpdateAnimator(BasicMountController MountController)
        {
            float forwardVel = Vector3.Dot(MountController.NavMesh.velocity.normalized, MountController.transform.forward);
            float sideVel = Vector3.Dot(MountController.NavMesh.velocity.normalized, MountController.transform.right);

            //dont even use X its just there because 
            MountController.Animator.SetFloat("Move X", sideVel, 5f, 5f);
            MountController.Animator.SetFloat("Move Z", forwardVel, 5f, 5f);
        }
    }
}
