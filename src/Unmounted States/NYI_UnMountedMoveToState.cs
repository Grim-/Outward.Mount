using SideLoader;
using UnityEngine;
using UnityEngine.AI;

namespace EmoMount
{
    public class NYI_UnMountedMoveToState : BaseUnMountedState
    {
        private Vector3 MoveToTarget = Vector3.zero;
        private NavMeshPath NavMeshPath = new NavMeshPath();

        public NYI_UnMountedMoveToState(Vector3 moveToTarget)
        {
            MoveToTarget = moveToTarget;
        }

        public override void OnEnter(BasicMountController MountController)
        {
            MountController.EnableNavMeshAgent();

            if (!MountController.NavMesh.CalculatePath(MoveToTarget, NavMeshPath))
            {
                EmoMountMod.Log.LogMessage("No valid path to target returning to last state");
                Parent.PopState();
            }
            else
            {
                MountController.DisplayNotification($"{MountController.MountName}! Go!");
                MountController.NavMesh.SetPath(NavMeshPath);
                MountController.NavMesh.isStopped = false;
            }
        }

        public override void OnExit(BasicMountController MountController)
        {
            MountController.DisplayNotification($"{MountController.MountName}, to me.");
        }

        public override void OnFixedUpdate(BasicMountController MountController)
        {
            
        }

        public override void OnUpdate(BasicMountController MountController)
        {
            base.OnUpdate(MountController);

            float DistanceFromTarget = Vector3.Distance(MountController.transform.position, MoveToTarget);

            if (DistanceFromTarget < MountController.TargetStopDistance)
            {
                MountController.NavMesh.isStopped = true;
                Parent.PopState();
            }

            if (CustomKeybindings.GetKeyDown("TEST_FOLLOWWAIT_TOGGLE_BUTTON") || MountController.DistanceToOwner > 50f)
            {
                Parent.PopState();
            }

        }

    }
}
