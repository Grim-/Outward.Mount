using UnityEngine;
using UnityEngine.AI;

namespace EmoMount
{
    public class NYI_UnMountedFetchItemState : BaseUnMountedState
    {
        private Item TargetItem = null;
        private NavMeshPath NavMeshPath = new NavMeshPath();

        public NYI_UnMountedFetchItemState(Item targetItem)
        {
            TargetItem = targetItem;
        }

        public override void OnEnter(BasicMountController MountController)
        {
            MountController.EnableNavMeshAgent();

            if (!MountController.NavMesh.CalculatePath(TargetItem.transform.position, NavMeshPath))
            {
                EmoMountMod.Log.LogMessage("No valid path to target item returning to last state");
                Parent.PopState();
            }
            else
            {
                MountController.DisplayNotification($"{MountController.MountName}! Fetch!");
                MountController.NavMesh.SetPath(NavMeshPath);
                MountController.NavMesh.isStopped = false;
            }
        }

        public override void OnExit(BasicMountController MountController)
        {
          
        }

        public override void OnFixedUpdate(BasicMountController MountController)
        {
           
        }

        public override void OnUpdate(BasicMountController MountController)
        {
            base.OnUpdate(MountController);

            if (TargetItem != null)
            {
                float DistanceFromTarget = Vector3.Distance(MountController.transform.position, TargetItem.transform.position);


                if (DistanceFromTarget < MountController.TargetStopDistance)
                {
                    MountController.AddItemToBag(TargetItem);
                    Parent.PopState();
                }
            }
        }

    }
}
