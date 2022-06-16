using UnityEngine;

namespace EmoMount
{

    /// <summary>
    /// Base UnMounted State only 
    /// </summary>
    public class BaseUnMountedState : BaseState<BasicMountController>
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
        }

        public void UpdateAnimator(BasicMountController MountController)
        {
            float forwardVel = Vector3.Dot(MountController.NavMesh.velocity.normalized, MountController.transform.forward);
            float sideVel = Vector3.Dot(MountController.NavMesh.velocity.normalized, MountController.transform.right);

            MountController.Animator.SetFloat("Move X", sideVel, 5f, 5f);
            MountController.Animator.SetFloat("Move Z", forwardVel, 5f, 5f);
        }
    }
}
