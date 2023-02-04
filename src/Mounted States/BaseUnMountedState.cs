using EmoMount.Mounted_States;
using UnityEngine;

namespace EmoMount
{

    /// <summary>
    /// Base UnMounted State only 
    /// </summary>
    //public class BaseUnMountedState : BaseMountState
    //{
    //    public override void OnEnter(BasicMountController MountController)
    //    {
          
    //    }

    //    public override void OnExit(BasicMountController MountController)
    //    {
          
    //    }

    //    public override void OnFixedUpdate(BasicMountController MountController)
    //    {
           
    //    }

    //    //public override void OnUpdate(BasicMountController MountController)
    //    //{
    //    //    if (!MountController.NavMesh.isOnNavMesh)
    //    //    {
    //    //        return;
    //    //    }

    //    //    base.OnUpdate(MountController);
    //    //}


    //    public override void UpdateAnimator(BasicMountController MountController)
    //    {
    //        float forwardVel = Vector3.Dot(MountController.NavMesh.velocity.normalized, MountController.transform.forward);
    //        float sideVel = Vector3.Dot(MountController.NavMesh.velocity.normalized, MountController.transform.right);

    //        //dont even use X its just there because 
    //        MountController.Animator.SetFloat("Move X", sideVel, 5f, 5f);
    //        MountController.Animator.SetFloat("Move Z", forwardVel, 5f, 5f);
    //    }
    //}
}
