using SideLoader;

namespace EmoMount
{
    public class NYI_UnMountedStayState : UnMountedState
    {
        public override void OnEnter(BasicMountController MountController)
        {
            MountController.DisplayNotification($"{MountController.MountName}, stay.");
            MountController.NavMesh.isStopped = true;
            MountController.DisableNavMeshAgent();
        }

        public override void OnExit(BasicMountController MountController)
        {
            MountController.NavMesh.isStopped = false;
            MountController.EnableNavMeshAgent();
        }

        public override void OnFixedUpdate(BasicMountController MountController)
        {
           
        }

        public override void OnUpdate(BasicMountController MountController)
        {
            if (CustomKeybindings.GetKeyDown("TEST_FOLLOWWAIT_TOGGLE_BUTTON") || MountController.DistanceToOwner > 50f)
            {
                MountController.DisplayNotification($"{MountController.MountName}, to me.");
                Parent.PopState();
            }

        }
    }
}
